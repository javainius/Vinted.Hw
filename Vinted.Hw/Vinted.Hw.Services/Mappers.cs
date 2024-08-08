﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vinted.Hw.Entities;
using Vinted.Hw.Models;

namespace Vinted.Hw.Services
{
    public static class Mappers
    {
        public static List<ShippingPriceModel> ShippingPriceTermEntitiesToShippingPriceTermModels(this List<ShippingPriceEntity> shippingPriceTermEntities)
        {
            return shippingPriceTermEntities.Select(x => x.ShippingPriceTermEntityToShippingPriceTermModel()).ToList();
        }

        public static ShippingPriceModel ShippingPriceTermEntityToShippingPriceTermModel(this ShippingPriceEntity shippingPriceTermEntity)
        {
            return new()
            {
                CarrierCode = shippingPriceTermEntity.Provider,
                PackageSize = (Models.PackageSize)shippingPriceTermEntity.PackageSize,
                Price = shippingPriceTermEntity.Price
            };
        }

        public static List<TransactionModel> TransactionEntitiesToTransactionModels(this List<TransactionEntity> transactionEntities)
        {
            return transactionEntities.Select(x => x.TransactionEntityToTransactionModel()).ToList();
        }

        public static TransactionModel TransactionEntityToTransactionModel(this TransactionEntity transactionEntity)
        {
            return new()
            {
                TransactionLine = transactionEntity.TransactionLine,
                IsIgnored = transactionEntity.IsIgnored,
                TransactionData = transactionEntity.TransactionData.TransactionDataEntityToTransactionDataModel()
            };
        }

        public static TransactionDataModel TransactionDataEntityToTransactionDataModel(this TransactionDataEntity transactionDataEntity)
        {
            return new()
            {
                CarrierCode = transactionDataEntity.CarrierCode,
                Date = transactionDataEntity.Date,
                PackageSize = (Models.PackageSize)transactionDataEntity.PackageSize,
                Price = transactionDataEntity.Price,
                Discount = transactionDataEntity.Discount,
            };
        }

        public static AccumulatedDiscountTermModel AccumulatedDiscountTermEntityToAccumulatedDiscountTermModel(this AccumulatedDiscountTermEntity accumulatedDiscountTermEntity)
        {
            return new()
            {
                DiscountAmount = accumulatedDiscountTermEntity.DiscountAmount
            };
        }

        public static FreeShipmentTermModel FreeShipmentTermEntityToFreeShippmentTermModel(this FreeShipmentTermEntity freeShippmentTermEntity)
        {
            return new()
            {
                PackageSize = (Models.PackageSize)freeShippmentTermEntity.PackageSize,
                CarrierCode = freeShippmentTermEntity.CarrierCode,
                TimesPerMonth = freeShippmentTermEntity.TimesPerMonth,
                WhichEveryShipment = freeShippmentTermEntity.WhichEveryShipment
            };
        }

        public static TransactionEntity TransactionModelToTransactionEntity(this TransactionModel transactionModel)
        {
            return new()
            {
                TransactionLine = transactionModel.TransactionLine,
                IsIgnored = transactionModel.IsIgnored,
                TransactionData = transactionModel.TransactionData.TransactionDataModelToTransactionDataEntity()
            };
        }

        public static TransactionDataEntity TransactionDataModelToTransactionDataEntity(this TransactionDataModel transactionDataModel)
        {
            return new()
            {
                CarrierCode = transactionDataModel.CarrierCode,
                Date = transactionDataModel.Date,
                PackageSize = (Entities.PackageSize)transactionDataModel.PackageSize,
                Price = transactionDataModel.Price,
                Discount = transactionDataModel.Discount,
            };
        }

        public static Models.PackageSize StringToPackageSize(string packageSize)
        {
            return packageSize switch
            {
                "S" => Models.PackageSize.Small,
                "M" => Models.PackageSize.Medium,
                "L" => Models.PackageSize.Large,
                _ => throw new ArgumentException($"Invalid package size: {packageSize}")
            };
        }

        public static string PackageSizeToString(Models.PackageSize packageSize)
        {
            return packageSize switch
            {
                Models.PackageSize.Small => "S",
                Models.PackageSize.Medium => "M",
                Models.PackageSize.Large => "L",
            };
        }
    }
}
