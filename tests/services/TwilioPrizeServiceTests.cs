using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using shared.Models;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tests.services
{
    [TestClass]
    public class TwilioPrizeServiceTests
    {
        private static TestContext _testContext;
        private Mock<IConfiguration> _configMock;
        private Mock<ILogger<PrizeService>> _loggerMock;
        private Mock<IPrizeStorageService> _storageMock;

        public TwilioPrizeServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<PrizeService>>();
            _storageMock = new Mock<IPrizeStorageService>();
        }

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddPrize_WithInvalidInfo_ThrowsArgumentException()
        {
            // ARRANGE
            _storageMock
                .Setup(x => x.AddItemToRepository(It.IsAny<RafflePrize>()))
                .ReturnsAsync(0);
            _storageMock.SetupProperty(x => x.IsInitialized).SetReturnsDefault(true);
            var svc = new PrizeService(_storageMock.Object, _loggerMock.Object);
            var prize = new RafflePrize { Quantity = 0 };

            // ACT
            await svc.InitializeService();
            var result = await svc.AddRafflePrize(prize);

            // ASSERT   
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await svc.AddRafflePrize(prize));
        }

        [TestMethod]
        public async Task AddPrize_WithValidInfo_ReturnsSuccessful()
        {
            // ARRANGE
            _storageMock
                .Setup(x => x.AddItemToRepository(It.IsAny<RafflePrize>()))
                .ReturnsAsync(0);
            _storageMock.SetupProperty(x => x.IsInitialized).SetReturnsDefault(true);
            var svc = new PrizeService(_storageMock.Object, _loggerMock.Object);
            var prize = new RafflePrize { Name = "Test Prize", Quantity = 1 };

            // ACT
            await svc.InitializeService();
            var result = await svc.AddRafflePrize(prize);

            // ASSERT
            Assert.IsTrue(result == RafflePrizeStatus.Successful);
        }

        [TestMethod]
        public async Task GetCurrentPrize_ReturnCurrentPrize()
        {
            // arrange
            _storageMock
               .Setup(x => x.AddItemToRepository(It.IsAny<RafflePrize>()))
               .ReturnsAsync(0);
            _storageMock.SetupProperty(x => x.IsInitialized).SetReturnsDefault(true);

            var prize = new RafflePrize { Name="Current Prize", Quantity=1, IsSelectedPrize = true };
            var svc = new PrizeService(_storageMock.Object, _loggerMock.Object);
            await svc.InitializeService();
            var result = await svc.AddRafflePrize(prize);

            // act
            var selected = await svc.GetCurrentPrize();

            // assert
            Assert.IsNotNull(selected);
            Assert.AreEqual(prize.Name, "Current Prize");
        }
    }
}
