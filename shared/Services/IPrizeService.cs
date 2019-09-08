using shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shared.Services
{
    public interface IPrizeService
    {
        bool IsInitialized { get; set; }
        Task InitializeService();
        Task<RafflePrizeStatus> AddRafflePrize(RafflePrize prize);
        Task<RafflePrize> GetCurrentPrize();
        Task<RafflePrize> SelectPrize();
        Task<IList<RafflePrize>> GetRafflePrizes();
    }
}