using AutoFixture;
using Moq;
using System.Collections.Generic;
using Vinted.Hw.Entities;
using Vinted.Hw.Models;
using Vinted.Hw.Persistence.Interfaces;
using Vinted.Hw.Services;

namespace Vinted.Hw.UnitTests
{
    [TestClass]
    public class ParsingServiceTests
    {
        private Fixture _fixture;
        private ParsingService _parsingService;
        private Mock<IShippingPriceRepository> _mockShippingServiceRepository;


        [TestInitialize]
        public void TestInitialize()
        {
            _mockShippingServiceRepository = new Mock<IShippingPriceRepository>();
            _parsingService = new ParsingService(_mockShippingServiceRepository.Object);
            _fixture = new Fixture();
        }

        [TestMethod]         
        public void GivenValidTransactionLines_WhenParseTransactions_ThenTransactionLinesAreReturned()
        {
            // Arrange
            List<ShippingPriceEntity> expectedShippingPriceEntities = GetShippingPriceFixtures();
            List<TransactionModel> expectedTransactions = expectedShippingPriceEntities
                .Select(x => GetExpectedTransactionModel(x, new DateOnly(2012,12,12)))
                .ToList();
            List<string> transactionLines = expectedTransactions.Select(x => x.TransactionLine).ToList();

            _mockShippingServiceRepository.Setup(x => x.GetShippingPriceTerms())
                                          .Returns(expectedShippingPriceEntities);

            // Act
            List<TransactionModel> actualTransactions = _parsingService.ParseTransactions(transactionLines);

            // Assert
            for (int i = 0; i < expectedTransactions.Count; i++)
            {
                TransactionModel expectedTransactionLine = expectedTransactions[i];
                TransactionModel actualTransactionLine = actualTransactions[i];

                Assert.AreEqual(expectedTransactionLine.TransactionLine, actualTransactionLine.TransactionLine);
                Assert.AreEqual(expectedTransactionLine.IsIgnored, actualTransactionLine.IsIgnored);

                Assert.IsNotNull(actualTransactionLine.TransactionData);
                Assert.AreEqual(expectedTransactionLine.TransactionData.Date, actualTransactionLine.TransactionData.Date);
                Assert.AreEqual(expectedTransactionLine.TransactionData.PackageSize, actualTransactionLine.TransactionData.PackageSize);
                Assert.AreEqual(expectedTransactionLine.TransactionData.CarrierCode, actualTransactionLine.TransactionData.CarrierCode);
                Assert.AreEqual(expectedTransactionLine.TransactionData.Price, actualTransactionLine.TransactionData.Price);
                Assert.AreEqual(expectedTransactionLine.TransactionData.Discount, actualTransactionLine.TransactionData.Discount);
            }

            _mockShippingServiceRepository.Verify(x => x.GetShippingPriceTerms(), Times.Exactly(expectedTransactions.Count));
        }

        //[TestMethod]
        //public void GivenInvalidTransactionLines_WhenParseTransactions_ThenIngoredTransactionLinesAreReturned()
        //{
        //    // Arrange
        //    List<ShippingPriceEntity> expectedShippingPriceEntities = GetShippingPriceFixtures();
        //    List<TransactionModel> expectedTransactions = expectedShippingPriceEntities
        //        .Select(x => GetExpectedTransactionModel(x, new DateOnly(2012, 12, 12)))
        //        .ToList();
        //    List<string> transactionLines = expectedTransactions.Select(x => x.TransactionLine).ToList();

        //    _mockShippingServiceRepository.Setup(x => x.GetShippingPriceTerms())
        //                                  .Returns(expectedShippingPriceEntities);

        //    // Act
        //    List<TransactionModel> actualTransactions = _parsingService.ParseTransactions(transactionLines);

        //    // Assert
        //    for (int i = 0; i < expectedTransactions.Count; i++)
        //    {
        //        TransactionModel expectedTransactionLine = expectedTransactions[i];
        //        TransactionModel actualTransactionLine = actualTransactions[i];

        //        Assert.AreEqual(expectedTransactionLine.TransactionLine, actualTransactionLine.TransactionLine);
        //        Assert.AreEqual(expectedTransactionLine.IsIgnored, actualTransactionLine.IsIgnored);

        //        Assert.IsNotNull(actualTransactionLine.TransactionData);
        //        Assert.AreEqual(expectedTransactionLine.TransactionData.Date, actualTransactionLine.TransactionData.Date);
        //        Assert.AreEqual(expectedTransactionLine.TransactionData.PackageSize, actualTransactionLine.TransactionData.PackageSize);
        //        Assert.AreEqual(expectedTransactionLine.TransactionData.CarrierCode, actualTransactionLine.TransactionData.CarrierCode);
        //        Assert.AreEqual(expectedTransactionLine.TransactionData.Price, actualTransactionLine.TransactionData.Price);
        //        Assert.AreEqual(expectedTransactionLine.TransactionData.Discount, actualTransactionLine.TransactionData.Discount);
        //    }

        //    _mockShippingServiceRepository.Verify(x => x.GetShippingPriceTerms(), Times.Exactly(expectedTransactions.Count));
        //}

        private List<ShippingPriceEntity> GetShippingPriceFixtures()
        {
            List<ShippingPriceEntity> expectedShippingPriceModel = GetShippingPriceFixtures(PackageSize.Small);
            expectedShippingPriceModel.AddRange(GetShippingPriceFixtures(PackageSize.Medium));
            expectedShippingPriceModel.AddRange(GetShippingPriceFixtures(PackageSize.Large));

            return expectedShippingPriceModel;
        }

        private List<ShippingPriceEntity> GetShippingPriceFixtures(PackageSize packageSize)
        {
            return _fixture.Build<ShippingPriceEntity>()
                .With(x => x.PackageSize, packageSize.PackageSizeToString())
                .CreateMany().ToList();
        }

        private TransactionModel GetExpectedTransactionModel(ShippingPriceEntity ShippingPriceEntity, DateOnly date)
        {
            TransactionDataModel transactionData = _fixture
                .Build<TransactionDataModel>()
                .With(x => x.PackageSize, ShippingPriceEntity.PackageSize.StringToPackageSize)
                .With(x => x.Date, date)
                .With(x => x.CarrierCode, ShippingPriceEntity.CarrierCode)
                .Without(x => x.Price)
                .Without(x => x.Discount)
                .Create();

            string transactionLine = $"{transactionData.Date.ToString()} " +
                    $"{ShippingPriceEntity.PackageSize} " +
                    $"{transactionData.CarrierCode}";

            return new()
            {
                IsIgnored = false,
                TransactionLine = transactionLine,
                TransactionData = transactionData
            };
        }

    }
}