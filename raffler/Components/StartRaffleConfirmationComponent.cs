using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class StartRaffleConfirmationComponent : ComponentBase
    {
        [Inject] private IRaffleService RaffleService { get; set; }
        [Inject] private IModalService Modal { get; set; }

        public async Task StartRaffleWithEntrants()
        {
            await RaffleService.StartRaffle(true);
            Modal.Close(ModalResult.Ok(this));
            StateHasChanged();
        }

        public async Task StartRaffleWithoutEntrants()
        {
            await RaffleService.StartRaffle(false);
            Modal.Close(ModalResult.Ok(this));
            StateHasChanged();
        }
    }
}
