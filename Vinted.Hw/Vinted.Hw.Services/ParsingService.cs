using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Entities;
using Vinted.Hw.Models;
using Vinted.Hw.Persistence.Interfaces;
using Vinted.Hw.Services.Interfaces;

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

            if (transactionLineParts.Length == 3)
            {
                try
                {
                    DateOnly date = DateOnly.Parse(transactionLineParts[0]);
                    PackageSize packageSize = transactionLineParts[1].StringToPackageSize();
                    string carrierCode = transactionLineParts[2];

                    ValidateCarrierCode(carrierCode);

                    transactionModel.TransactionData = new TransactionDataModel
                    {
                        Date = date,
                        PackageSize = packageSize,
                        CarrierCode = carrierCode
                    };

                    transactionModel.IsIgnored = false;
                }
                catch (Exception ex)
                {
                }
            }

            return transactionModel;
        }

        private void ValidateCarrierCode(string provider)
        {
            List<ShippingPriceModel> shippingPriceTermModels = _shippingPriceRepository
                .GetShippingPriceTerms()
                .ShippingPriceTermEntitiesToShippingPriceTermModels();

            foreach (ShippingPriceModel shippingPriceTermEntity in shippingPriceTermModels)
            {
                if (shippingPriceTermEntity.CarrierCode == provider)
                {
                    return;
                }
            }

            throw new Exception("such carrier doesn't exist");
        }
    }
}
