using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class PrizeComponent : ComponentBase
    {
        [Inject]
        private IPrizeService PrizeService { get; set; }

        [Inject]
        private IModalService ModalService { get; set; }

        public RafflePrize Prize { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Prize = new RafflePrize();
            if (!PrizeService.IsInitialized)
            {
                await PrizeService.InitializeService();
            }
        }

        public async Task AddNewPrize()
        {
            await PrizeService.AddRafflePrize(Prize);
            ModalService.Close(ModalResult.Ok(Prize));
        }
    }
}
