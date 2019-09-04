using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using shared.Models;
using shared.Services;
using System;
using System.Threading.Tasks;

namespace tests.services
{
    [TestClass]
    public class TwilioRaffleServiceTests
    {
        private Mock<IConfiguration> _configMock;
        private Mock<IRaffleStorageService> _updaterMock;
        private Mock<IPrizeService> _prizeServiceMock;
        private Mock<ILogger<RaffleService>> _loggerMock;

        private RaffleEntry _currentRaffleEntry;
        private Raffle _currentRaffle;

        [TestInitialize]
        public void InitializeTest()
        {
            _configMock = new Mock<IConfiguration>();
            _updaterMock = new Mock<IRaffleStorageService>();
            _prizeServiceMock = new Mock<IPrizeService>();
            _loggerMock = new Mock<ILogger<RaffleService>>();

            _currentRaffleEntry = new  RaffleEntry();
            _currentRaffle = new Raffle();
        }

        [TestMethod]
        public async Task StartRaffleAsync_CurrentRaffleNotRunning_ReturnsValidId()
        {
            // arrange
            _currentRaffle.State = RaffleState.NotRunning;
            _updaterMock
                .Setup(updater => updater.CreateRaffle(It.IsAny<string>(), It.IsAny<Raffle>()))
                .ReturnsAsync(new object());
            var svc = new RaffleService(_updaterMock.Object, _prizeServiceMock.Object, _configMock.Object, _loggerMock.Object)
            {
                LatestRaffle =
            _currentRaffle
            };

            // act
            await svc.StartRaffle();

            // assert
            Assert.IsTrue(_currentRaffle.State == RaffleState.Running);
        }

        [TestMethod]
        public async Task StartRaffleAsync_CurrentRaffleRunning_ThrowsApplicationException()
        {
            // arrange
            _currentRaffle.State = RaffleState.Running;
            var svc = new RaffleService(_updaterMock.Object, _prizeServiceMock.Object, _configMock.Object, _loggerMock.Object)
            {
                LatestRaffle =
            _currentRaffle
            };

            // act
            // assert
            await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await svc.StartRaffle());
        }

        [TestMethod]
        public async Task AddRaffleEntry_ToRunningRaffle_WithValidEntry_ReturnsValidId()
        {
            // arrange
            _currentRaffle.State = RaffleState.Running;
            _updaterMock
                .Setup(updater =>
                    updater.UpdateRaffle(It.IsAny<Raffle>()))
                .Returns(async () => await Task.CompletedTask);
            var svc = new RaffleService(_updaterMock.Object, _prizeServiceMock.Object, _configMock.Object, _loggerMock.Object)
            {
                LatestRaffle = _currentRaffle
            };

            // act
            await svc.AddRaffleEntry(_currentRaffleEntry);

            // assert
            Assert.IsTrue(_currentRaffle.Entries.Count == 1);
            Assert.AreEqual(_currentRaffle.Entries[0], _currentRaffleEntry);
        }

        [TestMethod]
        public async Task AddRaffleEntry_ToNotRunningRaffle_WithValidEntry_ReturnNull()
        {
            // arrange
            _currentRaffle = new Raffle { State = RaffleState.NotRunning };
            _currentRaffleEntry = new RaffleEntry();
            var svc = new RaffleService(_updaterMock.Object, _prizeServiceMock.Object, _configMock.Object, _loggerMock.Object)
            {
                LatestRaffle = _currentRaffle
            };

            // act
            var result = await svc.AddRaffleEntry(_currentRaffleEntry);

            // assert
            // TODO: Update ASSERT for Adding a Raffle Entry to a Not Running
            Assert.IsNotNull(result);
        }

        [TestCleanup]
        public void CleanupTest()
        {
            _currentRaffleEntry = null;
            _currentRaffle = null;

            _configMock = null;
            _updaterMock = null;
        }
    }
}
