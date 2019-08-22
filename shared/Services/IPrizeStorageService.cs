using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    //TODO: Replace with a factory which can resolve the storage update service and rename to IStorageUpdater
    public interface IPrizeStorageService
    {
        Task<object> CreateStorage(string serviceId, string raffleName, object data);
        Task UpdateStorage(string serviceId, string id, object data);
        Task<object> FindItemByName(string serviceId, string itemName);
        Task<IList<RafflePrize>> GetItems();
    }
}
