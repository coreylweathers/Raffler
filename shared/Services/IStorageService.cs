using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IStorageService
    {
        Task<string> AddRaffleEntryAsync(RaffleEntry entry);
        Task<IEnumerable<RaffleEntry>> GetRaffleEntriesAsync();
    }
}
