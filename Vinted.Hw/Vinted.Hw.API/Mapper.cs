using Vinted.Hw.Contracts;
using Vinted.Hw.Models;
using Vinted.Hw.Services;

namespace Vinted.Hw.API
{
    public static class Mapper
    {
        public static List<TransactionContract> TransactionModelsToTransactionContracts(this List<TransactionModel> transactionModels)
        {
            return transactionModels.Select(x => x.TransactionModelToTransactionContract()).ToList();
        }

        public static TransactionContract TransactionModelToTransactionContract(this TransactionModel transactionModel)
        {
            return new()
            {
                TransactionLine = transactionModel.TransactionLine,
                IsIgnored = transactionModel.IsIgnored,
                TransactionData = transactionModel.TransactionData?.TransactionDataModelToTransactionDataContract()
            };
        }

        public static TransactionDataContract TransactionDataModelToTransactionDataContract(this TransactionDataModel transactionDataModel)
        {
            return new()
            {
                Date = transactionDataModel.Date,
                PackageSize = transactionDataModel.PackageSize.PackageSizeToString(),
                CarrierCode = transactionDataModel.CarrierCode,
                Price = transactionDataModel.Price,
                Discount = transactionDataModel.Discount
            };
        }
    }
}
