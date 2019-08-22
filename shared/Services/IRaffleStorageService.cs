using shared.Models;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IRaffleStorageService
    {
        Task<object> CreateRaffle(string raffleName, object data);
        Task UpdateRaffle(Raffle data);
        Task<object> FindRaffleItemByName(string itemName);
        Task ClearStorage();
        Task<Raffle> GetLatestRaffle();
    }
}
