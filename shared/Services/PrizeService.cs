using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using shared.Models;

namespace shared.Services
{
    public class PrizeService : IPrizeService
    {
        private readonly IPrizeStorageService _storageUpdater;

        public PrizeService(IPrizeStorageService storageUpdater) => _storageUpdater = storageUpdater;
        
        public Task AddRafflePrize()
        {
            throw new NotImplementedException();
        }
        
        public Task<RafflePrize> GetCurrentPrize()
        {
            throw new NotImplementedException();
        }

        public Task<RafflePrize> SelectPrize()
        {
            throw new NotImplementedException();
        }

        public Task<IList<RafflePrize>> GetRafflePrizes()
        {
            throw new NotImplementedException();
        }
    }
}