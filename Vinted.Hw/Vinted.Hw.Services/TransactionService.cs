using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vinted.Hw.API;
using Vinted.Hw.Entities;
using Vinted.Hw.Models;
using Vinted.Hw.Persistence.Interfaces;
using Vinted.Hw.Services.Interfaces;

namespace Vinted.Hw.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IParsingService _parsingService;
        private readonly IShippingPriceRepository _shippingPriceRepository;
        private readonly IAccumulatedDiscountTermRepository _accumulatedDiscountTermRepository;
        private readonly IFreeShipmentTermRepository _freeShipmentTermRepository;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IShippingPriceRepository shippingPriceRepository,
            IParsingService parsingService,
            IAccumulatedDiscountTermRepository accumulatedDiscountTermRepository,
            IFreeShipmentTermRepository freeShipmentTermRepository
            )
        {
            _transactionRepository = transactionRepository;
            _parsingService = parsingService;
            _shippingPriceRepository = shippingPriceRepository;
            _accumulatedDiscountTermRepository = accumulatedDiscountTermRepository;
            _freeShipmentTermRepository = freeShipmentTermRepository;
        }

        public async Task<List<TransactionModel>> GetProcessedTransactions(IFormFile transactionsFile)
        {
            List<string> transactionLines = await FileReader.GetTransactionLines(transactionsFile);

            List<TransactionModel> transactions = _parsingService.ParseTransactions(transactionLines);
            List<TransactionModel> processedTransactions = new();

            foreach (TransactionModel transaction in transactions)
            {
                if (!transaction.IsIgnored)
                {
                    transaction.TransactionData = ProcessTransactionData(transaction.TransactionData);
                }

                TransactionEntity savedTransaction = _transactionRepository
                    .SaveTransaction(transaction.TransactionModelToTransactionEntity());
                processedTransactions.Add(savedTransaction.TransactionEntityToTransactionModel());

            }

            return processedTransactions;
        }

        public TransactionDataModel ProcessTransactionData(TransactionDataModel transaction)
        {
            transaction = ApplyBasePrice(transaction);

            List<TransactionModel> currentMonthTransactions = GetCurrentMonthTransactions(transaction.Date);
            double currentMonthDiscountBalance = GetMonthlyAccumulatedDiscountBalance(currentMonthTransactions);

            return ApplyDiscount(transaction, currentMonthTransactions, currentMonthDiscountBalance); ;
        }

        private List<TransactionModel> GetCurrentMonthTransactions(DateOnly transactionDate)
        {
            List<TransactionModel> allTransactions = _transactionRepository
                .GetTransactions()
                .TransactionEntitiesToTransactionModels();

            return allTransactions
                .Where(x => !x.IsIgnored 
                && x.TransactionData.Date.Year == transactionDate.Year 
                && x.TransactionData.Date.Month == transactionDate.Month)
                .ToList();
        }

        private TransactionDataModel ApplyBasePrice(TransactionDataModel transaction)
        {
            List<ShippingPriceModel> shippingPrices = _shippingPriceRepository
                .GetShippingPriceTerms()
                .ShippingPriceTermEntitiesToShippingPriceTermModels();

            transaction.Price = shippingPrices
                .Where(x => x.CarrierCode == transaction.CarrierCode && x.PackageSize == transaction.PackageSize)
                .First()
                .Price;

            return transaction;
        }

        private TransactionDataModel ApplyDiscount(
            TransactionDataModel transaction,
            List<TransactionModel> currentMonthTransactions,
            double currentMonthDiscountBalance
            )
        {
            if (transaction.PackageSize == PackageSize.Small)
            {
                return ApplySmallPackageDiscount(transaction, currentMonthDiscountBalance);
            }

            if (IsFreeShipment(transaction, currentMonthTransactions))
            {
                return ApplyFreeShippingDiscount(transaction, currentMonthDiscountBalance);
            }

            return transaction;
        }

        private TransactionDataModel ApplySmallPackageDiscount(TransactionDataModel transaction, double currentMonthDiscountBalance)
        {
            List<ShippingPriceModel> shippingPriceModels = _shippingPriceRepository
                .GetShippingPriceTerms()
                .ShippingPriceTermEntitiesToShippingPriceTermModels();

            double lowestSPackagePrice = GetLowestSPackagePrice(shippingPriceModels);
            double sPackageDiscount = transaction.Price - lowestSPackagePrice;

            if (sPackageDiscount <= currentMonthDiscountBalance)
            {
                transaction.Price = lowestSPackagePrice;
                transaction.Discount = sPackageDiscount;
            }
            else
            {
                transaction.Price = transaction.Price - currentMonthDiscountBalance;
                transaction.Discount = currentMonthDiscountBalance;
            }

            return transaction;
        }

        private static TransactionDataModel ApplyFreeShippingDiscount(TransactionDataModel transaction, double currentMonthDiscountBalance)
        {
            if (transaction.Price < currentMonthDiscountBalance)
            {
                transaction.Discount = transaction.Price;
                transaction.Price = 0;
            }
            else
            {
                transaction.Price = transaction.Price - currentMonthDiscountBalance;
                transaction.Discount = currentMonthDiscountBalance;
            }

            return transaction;
        }

        private bool IsFreeShipment(TransactionDataModel transaction, List<TransactionModel> currentMonthTransactions)
        {
            FreeShipmentTermModel freeShipmentTerm = _freeShipmentTermRepository.GetFreeShipmentTerm().FreeShipmentTermEntityToFreeShippmentTermModel();

            if (transaction.PackageSize == freeShipmentTerm.PackageSize && transaction.CarrierCode == freeShipmentTerm.CarrierCode)
            {
                int suchConditionsShipmentNumber = currentMonthTransactions
                    .Where(x => x.TransactionData.PackageSize == freeShipmentTerm.PackageSize && x.TransactionData.CarrierCode == freeShipmentTerm.CarrierCode)
                    .Count() + 1;
                int numberOfFreeTransactions = currentMonthTransactions.Where(x => x.TransactionData.Price == 0).Count();

                if (numberOfFreeTransactions < freeShipmentTerm.TimesPerMonth && suchConditionsShipmentNumber % freeShipmentTerm.WhichEveryShipment == 0)
                {
                    return true;
                }
            }

            return false;
        }

        // had to use casting to decimal due to known C# floating point issue
        private double GetMonthlyAccumulatedDiscountBalance(List<TransactionModel> currentMonthTransactions)
        {
            decimal currentMonthDiscountBalance = (decimal)_accumulatedDiscountTermRepository
                .GetAccumulatedDiscountTerm()
                .AccumulatedDiscountTermEntityToAccumulatedDiscountTermModel()
                .DiscountAmount;

            foreach (TransactionModel curentMonthTransaction in currentMonthTransactions)
            {
                currentMonthDiscountBalance -= (decimal)curentMonthTransaction.TransactionData.Discount;
            }

            return (double)currentMonthDiscountBalance;
        }

        private double GetLowestSPackagePrice(List<ShippingPriceModel> shippingPriceModels)
        {
            if (shippingPriceModels == null || !shippingPriceModels.Any())
            {
                throw new ArgumentException("The list of shipping price terms cannot be null or empty.");
            }

            return shippingPriceModels.Where(x => x.PackageSize == PackageSize.Small).Min(x => x.Price);
        }
    }
}
