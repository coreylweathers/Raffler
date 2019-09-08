using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IRaffleService
    {
        bool IsInitialized { get; set; }
        Raffle LatestRaffle { get; set; }
        Task InitializeService();
        Task StopRaffle();
        Task StartRaffle(bool reuseEntrants = false);
        Task<string> AddRaffleEntry(RaffleEntry entry);
        Task<string> SelectRaffleWinner();
        Task ClearRaffles();
        Task<bool> ContainsPhoneNumber(string phoneNumber);
        Task<IEnumerable<RaffleWinner>> GetPreviousRaffleWinners();
    }
}
