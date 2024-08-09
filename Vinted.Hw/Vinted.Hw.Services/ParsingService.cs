using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Entities;
using Vinted.Hw.Models;
using Vinted.Hw.Persistence;

namespace Vinted.Hw.Services
{
    public class ParsingService : IParsingService
    {
        private readonly IShippingPriceRepository _shippingPriceRepository;

        public ParsingService(IShippingPriceRepository shippingPriceRepository)
        {
            _shippingPriceRepository = shippingPriceRepository;
        }

        public List<TransactionModel> ParseTransactions(List<string> transactionLines)
        {
            List<TransactionModel> parsedTransactions = new();

            foreach (string line in transactionLines)
            {             
                parsedTransactions.Add(GetParsedTransaction(line));
            }

            return parsedTransactions;
        }

        private TransactionModel GetParsedTransaction(string transactionLine)
        {
            TransactionModel transactionModel = new()
            {
                TransactionLine = transactionLine,
                IsIgnored = true
            };

            string[] transactionLineParts = transactionLine.Split(' ');

            if (IsLineValid(transactionLineParts))
            {
                transactionModel.IsIgnored = false;
                transactionModel.TransactionData = new TransactionDataModel
                {
                    Date = DateOnly.Parse(transactionLineParts[0]),
                    PackageSize = transactionLineParts[1].StringToPackageSize(),
                    CarrierCode = transactionLineParts[2]
                };
            }

            return transactionModel;
        }

        private bool IsLineValid(string[] lineParts) => lineParts.Length == 3 && IsProviderValid(lineParts[2]);

        private bool IsProviderValid(string provider)
        {
            List<ShippingPriceModel> shippingPriceTermModels = _shippingPriceRepository
                .GetShippingPriceTerms()
                .ShippingPriceTermEntitiesToShippingPriceTermModels();

            foreach(ShippingPriceModel shippingPriceTermEntity in shippingPriceTermModels)
            {
                if (shippingPriceTermEntity.CarrierCode == provider)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
