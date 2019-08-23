using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    //TODO: Replace with a factory which can resolve the storage update service and rename to IStorageUpdater
    public interface IPrizeStorageService
    {
        bool IsInitialized { get; set; }
        Task InitializeService();
        Task<object> CreateRepository();
        Task<int> AddItemToRepository(RafflePrize data);
        Task UpdateRepository(int index, RafflePrize data);
        Task<RafflePrize> FindItemByName(string itemName);
        Task<IDictionary<int, RafflePrize>> GetItems();
    }
}
