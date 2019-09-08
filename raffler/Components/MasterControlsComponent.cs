using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class MasterControlsComponent : ComponentBase
    {
        [Inject] private IRaffleService RaffleService { get; set; }
        [Inject] private IModalService Modal { get; set; }


        private bool _isStartButtonEnabled;

        private bool _isStopButtonEnabled;

        private bool _isWinnerButtonEnabled;

        protected async Task StartRaffle()
        {
            await Task.FromException(new NotImplementedException());

            /*
            Console.WriteLine($"{DateTime.Now}: Starting the Raffle");
            Modal.Show("Confirm Raffle Start", typeof(raffler.Pages.StartRaffleConfirmation));
            Console.WriteLine($"{DateTime.Now}: Started the Raffle");
            //ToggleEnabledButtons(false, true, true);
            StateHasChanged();
            await Task.CompletedTask;*/
        }

        protected async Task StopRaffle()
        {
            await Task.FromException(new NotImplementedException());

            /*
            Console.WriteLine($"{DateTime.Now}: Stopping the Raffle");
            await RaffleService.StopRaffle();
            Console.WriteLine($"{DateTime.Now}: Stopped the Raffle");
            ToggleEnabledButtons(true, false, true);
            StateHasChanged();*/
        }

        protected async Task SelectRaffleWinner()
        {
            await Task.FromException(new NotImplementedException());
            /*
            Console.WriteLine($"{DateTime.Now}: Selecting a raffle winner");
            var result = await RaffleService.SelectRaffleWinner();
            Console.WriteLine($"{DateTime.Now}: Resulting SID from notifying winner - {result}");
            Console.WriteLine($"{DateTime.Now}: Selected a raffle winner");
            ToggleEnabledButtons(true, false, false);
            StateHasChanged();*/
        }

        private void ToggleEnabledButtons(bool enableStart, bool enableStop, bool enableWinner)
        {
            _isStartButtonEnabled = enableStart;
            _isStopButtonEnabled = enableStop;
            _isWinnerButtonEnabled = enableWinner;
        }
    }
}
