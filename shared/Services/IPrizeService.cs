using System.Collections.Generic;
using System.Threading.Tasks;
using shared.Models;

namespace shared.Services
{
    public interface IPrizeService
    {
        Task AddRafflePrize();
        Task<RafflePrize> GetCurrentPrize();
        Task<RafflePrize> SelectPrize();
        Task<IList<RafflePrize>> GetRafflePrizes();
    }
}