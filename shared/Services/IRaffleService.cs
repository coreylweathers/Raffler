using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IRaffleService 
    {
        
        Raffle CurrentRaffle { get; set; }
        Task<Raffle> GetCurrentRaffleAsync();
        Task<string> StopRaffleAsync();
        Task<string> StartRaffleAsync();
        Task<string> EndRaffleAsync();
        Task<string> AddRaffleEntryAsync(RaffleEntry entry);
        Task<IList<RaffleEntry>> GetRaffleEntriesAsync();
        Task<string> SelectRaffleWinnerAsync();
        Task ClearRafflesAsync();
    }
}
