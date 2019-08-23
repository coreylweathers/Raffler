using System.Collections.Generic;
using System.Threading.Tasks;
using shared.Models;

namespace shared.Services
{
    public interface IPrizeService
    {
        bool IsInitialized { get; set; }
        Task InitializeService();
        Task AddRafflePrize(RafflePrize prize);
        Task<RafflePrize> GetCurrentPrize();
        Task<RafflePrize> SelectPrize();
        Task<IList<RafflePrize>> GetRafflePrizes();
    }
}