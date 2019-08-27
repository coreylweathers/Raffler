using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IRaffleService 
    {
        Raffle LatestRaffle { get; set; }
        Task InitializeService();
        Task StopRaffle();
        Task StartRaffle();
        Task<string> AddRaffleEntry(RaffleEntry entry);
        Task<string> SelectRaffleWinner();
        Task ClearRaffles();
        Task<bool> ContainsPhoneNumber(string phoneNumber);
    }
}
