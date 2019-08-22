using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace raffler.Components
{
    public class RaffleComponent : ComponentBase
    {
        [Inject] private IRaffleService RaffleService { get; set; }
        [Inject] private IPrizeService PrizeService { get; set; }
        [Inject] private HubConnectionBuilder HubConnectionBuilder { get; set; }
        [Inject] private IUriHelper UriHelper { get; set; }

        protected readonly string RaffleNumber = "(913) 270-6063";
        protected List<RaffleEntry> EntryList { get; set; }

        private bool _isStartButtonEnabled;

        private bool _isStopButtonEnabled;

        private bool _isWinnerButtonEnabled;

        protected string PrizeName { get; private set; }

        protected string PrizeUrl { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await InitalizeSignalRAsync();
            if (RaffleService.LatestRaffle == null)
            {
                await RaffleService.InitializeService();
            }
            EntryList = RaffleService.LatestRaffle?.Entries?.ToList() ?? new List<RaffleEntry>();

            if (RaffleService.LatestRaffle != null)
            {
                _isStartButtonEnabled = RaffleService.LatestRaffle.State == RaffleState.NotRunning;
                _isStopButtonEnabled = !_isStartButtonEnabled;
                _isWinnerButtonEnabled = RaffleService.LatestRaffle.Entries.Any(entry => entry.IsWinner == false);
                Console.Write($"IsStartButton: {_isStartButtonEnabled}\r\nIsStopButton: {_isStopButtonEnabled}\r\nIsWinnerButton: {_isWinnerButtonEnabled}");

                if (RaffleService.LatestRaffle.Prize != null)
                {
                    PrizeName = RaffleService.LatestRaffle.Prize.Name;
                    PrizeUrl = RaffleService.LatestRaffle.Prize.ImageUrl;
                }
            }

        }

        private async Task UpdateEntryListAsync(RaffleEntry entry)
        {
            // TODO: Replace Console.WriteLine with ILogger
            Console.WriteLine($"{DateTime.Now}: Adding {entry.MessageSid} to the entry list");
            await Task.Run(() => EntryList.Add(entry));
            Console.WriteLine($"{DateTime.Now}: Adding {entry.MessageSid} to the entry list");
            ToggleEnabledButtons(false, true, true);
            StateHasChanged();
        }

        protected async Task StartRaffleAsync()
        {
            Console.WriteLine($"{DateTime.Now}: Starting the Raffle");
            await RaffleService.StartRaffle();
            EntryList.Clear();
            Console.WriteLine($"{DateTime.Now}: Started the Raffle");
            ToggleEnabledButtons(false, true, true);
            StateHasChanged();
        }

        protected async Task StopRaffleAsync()
        {
            Console.WriteLine($"{DateTime.Now}: Stopping the Raffle");
            await RaffleService.StopRaffle();
            Console.WriteLine($"{DateTime.Now}: Stopped the Raffle");
            ToggleEnabledButtons(true, false, true);
            StateHasChanged();
        }

        protected async Task ClearRafflesAsync()
        {
            await RaffleService.ClearRaffles();
        }

        protected async Task SelectRaffleWinnerAsync()
        {
            Console.WriteLine($"{DateTime.Now}: Selecting a raffle winner");
            var result = await RaffleService.SelectRaffleWinner();
            Console.WriteLine($"{DateTime.Now}: Resulting SID from notifying winner - {result}");
            Console.WriteLine($"{DateTime.Now}: Selected a raffle winner");
            ToggleEnabledButtons(true, false, false);
            StateHasChanged();
        }

        private void ToggleEnabledButtons(bool enableStart, bool enableStop, bool enableWinner)
        {
            _isStartButtonEnabled = enableStart;
            _isStopButtonEnabled = enableStop;
            _isWinnerButtonEnabled = enableWinner;
        }

        private async Task InitalizeSignalRAsync()
        {
            var uri = new Uri($"{UriHelper.GetBaseUri()}rafflehub");
            // in Component Initialization code
            var connection = HubConnectionBuilder // the injected one from above.
                    .WithUrl(uri) // The hub URL. If the Hub is hosted on the server where the blazor is hosted, you can just use the relative path.
                    .Build(); // Build the HubConnection
            connection.On<RaffleEntry>("AddRaffleEntry", UpdateEntryListAsync);
            await connection.StartAsync();
        }
    }
}
