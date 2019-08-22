using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using shared.Models;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace tests.services
{
    [TestClass]
    public class TwilioSyncServiceTests
    {
        private Mock<IConfiguration> _configMock;
        private Mock<IStorageUpdater> _updaterMock;

        [ClassInitialize]
        public void InitializeClass()
        {
            _configMock = new Mock<IConfiguration>();
            _updaterMock = new Mock<IStorageUpdater>();
        }

        [TestMethod]
        public async Task AddRaffleEntry_WithValidEntry_ReturnsValidId()
        {
            // arrange
            var currentRaffle = new Raffle { State = RaffleState.Running };
            var entry = new RaffleEntry { MessageSid = "7894728947328" };

            _updaterMock
                .Setup(updater => 
                    updater.UpdateStorage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Raffle>()))
                .Returns(async () => await Task.CompletedTask);
            var svc = new TwilioRaffleService(_updaterMock.Object, _configMock.Object) { CurrentRaffle =  currentRaffle};
            
            // act
            var result = await svc.AddRaffleEntryAsync(entry);

            // assert
            Assert.IsTrue(currentRaffle.Entries.Count == 1);
            Assert.AreEqual(currentRaffle.Entries[0], entry);
        }
        
        [TestMethod]
        public async 

        [ClassCleanup]
        public void TeardownClass()
        {
            _configMock = null;
            _updaterMock = null;
        }
    }
}
