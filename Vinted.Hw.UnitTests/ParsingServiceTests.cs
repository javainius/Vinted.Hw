using AutoFixture;
using Moq;
using System.Collections.Generic;
using Vinted.Hw.Entities;
using Vinted.Hw.Models;
using Vinted.Hw.Persistence.Interfaces;
using Vinted.Hw.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            List<ShippingPriceEntity> expectedShippingPriceEntities = GetShippingPriceTermFixtures();
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
                Assert.AreEqual(expectedTransactions[i].TransactionLine, actualTransactions[i].TransactionLine);
                Assert.AreEqual(expectedTransactions[i].IsIgnored, actualTransactions[i].IsIgnored);
                Assert.IsNotNull(actualTransactions[i].TransactionData);
                Assert.AreEqual(expectedTransactions[i].TransactionData.Date, actualTransactions[i].TransactionData.Date);
                Assert.AreEqual(expectedTransactions[i].TransactionData.PackageSize, actualTransactions[i].TransactionData.PackageSize);
                Assert.AreEqual(expectedTransactions[i].TransactionData.CarrierCode, actualTransactions[i].TransactionData.CarrierCode);
                Assert.AreEqual(expectedTransactions[i].TransactionData.Price, actualTransactions[i].TransactionData.Price);
                Assert.AreEqual(expectedTransactions[i].TransactionData.Discount, actualTransactions[i].TransactionData.Discount);
            }

            _mockShippingServiceRepository.Verify(x => x.GetShippingPriceTerms(), Times.Exactly(expectedTransactions.Count));
        }

        [TestMethod]
        public void GivenWrongStructureTransactionLines_WhenParseTransactions_ThenIngoredTransactionLinesAreReturned()
        {
            // Arrange
            List<TransactionModel> expectedTransactions = _fixture
                .Build<TransactionModel>()
                .With(x => x.IsIgnored, true)
                .Without(x => x.TransactionData)
                .CreateMany()
                .ToList();

            for (int i = 0; i < expectedTransactions.Count; i++)
            {
                expectedTransactions[i].TransactionLine = $"2015-02-0{i}S MR";
            }

            List<string> transactionLines = expectedTransactions.Select(x => x.TransactionLine).ToList();

            // Act
            List<TransactionModel> actualTransactions = _parsingService.ParseTransactions(transactionLines);

            // Assert
            AssertIgnoredTransactions(expectedTransactions, actualTransactions);

            _mockShippingServiceRepository.Verify(x => x.GetShippingPriceTerms(), Times.Never);
        }

        [TestMethod]
        public void GivenInvalidDateTransactionLines_WhenParseTransactions_ThenIgnoredTransactionLinesAreReturned()
        {
            // Arrange
            List<TransactionModel> expectedTransactions = _fixture
                .Build<TransactionModel>()
                .With(x => x.IsIgnored, true)
                .Without(x => x.TransactionData)
                .CreateMany()
                .ToList();

            for (int i = 0; i < expectedTransactions.Count; i++)
            {
                expectedTransactions[i].TransactionLine = $"2015-02-{i}00 S MR";
            }

            List<string> transactionLines = expectedTransactions.Select(x => x.TransactionLine).ToList();

            // Act
            List<TransactionModel> actualTransactions = _parsingService.ParseTransactions(transactionLines);

            // Assert
            AssertIgnoredTransactions(expectedTransactions, actualTransactions);

            _mockShippingServiceRepository.Verify(x => x.GetShippingPriceTerms(), Times.Never);
        }

        [TestMethod]
        public void GivenWrongPackageSizeTransactionLines_WhenParseTransactions_ThenIgnoredTransactionLinesAreReturned()
        {
            // Arrange
            List<TransactionModel> expectedTransactions = _fixture
                .Build<TransactionModel>()
                .With(x => x.IsIgnored, true)
                .Without(x => x.TransactionData)
                .CreateMany()
                .ToList();

            for (int i = 0; i < expectedTransactions.Count; i++)
            {
                expectedTransactions[i].TransactionLine = $"2015-02-0{i} R MR";
            }

            List<string> transactionLines = expectedTransactions.Select(x => x.TransactionLine).ToList();

            // Act
            List<TransactionModel> actualTransactions = _parsingService.ParseTransactions(transactionLines);

            // Assert
            AssertIgnoredTransactions(expectedTransactions, actualTransactions);

            _mockShippingServiceRepository.Verify(x => x.GetShippingPriceTerms(), Times.Never);
        }

        [TestMethod]
        public void GivenNotMatchingCarrierCodeTransactionLines_WhenParseTransactions_ThenIgnoredTransactionLinesAreReturned()
        {
            // Arrange
            List<TransactionModel> expectedTransactions = _fixture
                .Build<TransactionModel>()
                .With(x => x.IsIgnored, true)
                .Without(x => x.TransactionData)
                .CreateMany()
                .ToList();

            for (int i = 0; i < expectedTransactions.Count; i++)
            {
                expectedTransactions[i].TransactionLine = $"2015-02-{i}1 L NotExisting";
            }

            List<string> transactionLines = expectedTransactions.Select(x => x.TransactionLine).ToList();
            _mockShippingServiceRepository.Setup(x => x.GetShippingPriceTerms())
                              .Returns(GetShippingPriceTermFixtures());

            // Act
            List<TransactionModel> actualTransactions = _parsingService.ParseTransactions(transactionLines);

            // Assert
            AssertIgnoredTransactions(expectedTransactions, actualTransactions);

            _mockShippingServiceRepository
                .Verify(x => x.GetShippingPriceTerms(), Times.Exactly(expectedTransactions.Count));
        }

        private void AssertIgnoredTransactions(List<TransactionModel> expectedTransactions, List<TransactionModel> actualTransactions)
        {
            for (int i = 0; i < expectedTransactions.Count; i++)
            {
                Assert.AreEqual(expectedTransactions[i].TransactionLine, actualTransactions[i].TransactionLine);
                Assert.AreEqual(expectedTransactions[i].IsIgnored, actualTransactions[i].IsIgnored);
                Assert.IsNull(actualTransactions[i].TransactionData);
            }
        }

        private List<ShippingPriceEntity> GetShippingPriceTermFixtures()
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