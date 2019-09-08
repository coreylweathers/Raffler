using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class RafflePrizesComponent : ComponentBase
    {
        [Inject] private IPrizeService PrizeService { get; set; }
        protected List<RafflePrize> Prizes { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!PrizeService.IsInitialized)
            {
                await PrizeService.InitializeService();
            }

            Prizes = await PrizeService.GetRafflePrizes();
        }
    }
}
