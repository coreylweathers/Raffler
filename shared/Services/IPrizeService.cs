using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IPrizeService
    {
        List<RafflePrize> Prizes { get; }
        bool IsInitialized { get; set; }
        Task InitializeService();
        Task<RafflePrizeStatus> AddRafflePrize(RafflePrize prize);
        Task<RafflePrize> GetCurrentPrize();
        Task<RafflePrize> SelectPrize();
        Task<List<RafflePrize>> GetRafflePrizes();
    }
}