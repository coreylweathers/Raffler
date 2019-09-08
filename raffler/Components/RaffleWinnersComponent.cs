using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class RaffleWinnersComponent : ComponentBase
    {
        [Inject] protected IRaffleService RaffleService { get; set; }
        protected List<RaffleWinner> RaffleWinners { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!RaffleService.IsInitialized)
            {
                await RaffleService.InitializeService();
            }

            RaffleWinners = (await RaffleService.GetPreviousRaffleWinners()).ToList();

        }
    }
}
