using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.Modal.Services;
using System.Net.Http;

namespace raffler.Components
{
    public class RaffleComponent : ComponentBase
    {
        [Inject] private IRaffleService RaffleService { get; set; }
        [Inject] private IModalService Modal {get;set;} 
        [Inject] private HubConnectionBuilder HubConnectionBuilder { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected readonly string RaffleNumber = "(404) 800-3505";
        protected List<RaffleEntry> EntryList { get; set; }

        private bool _isStartButtonEnabled;

        private bool _isStopButtonEnabled;

        private bool _isWinnerButtonEnabled;

        protected string PrizeName { get; private set; }

        protected string PrizeUrl { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeServices();

            if (RaffleService.LatestRaffle == null)
            {
                return;
            }
            EntryList = RaffleService.LatestRaffle?.Entries?.ToList() ?? new List<RaffleEntry>();

            
            _isStartButtonEnabled = RaffleService.LatestRaffle.State == RaffleState.NotRunning;
            _isStopButtonEnabled = !_isStartButtonEnabled;
            _isWinnerButtonEnabled = RaffleService.LatestRaffle.Entries.Any(entry => entry.IsWinner == false);
            Console.Write($"IsStartButton: {_isStartButtonEnabled}\r\nIsStopButton: {_isStopButtonEnabled}\r\nIsWinnerButton: {_isWinnerButtonEnabled}");

            if (RaffleService.LatestRaffle.Prize == null)
            {
                return;
            }
            PrizeName = RaffleService.LatestRaffle.Prize.Name;
            PrizeUrl = RaffleService.LatestRaffle.Prize.ImageUrl;
        }

        private async Task InitializeServices()
        {
            await InitalizeSignalRAsync();
            if (RaffleService.LatestRaffle == null)
            {
                await RaffleService.InitializeService();
            }
        }

        private async Task UpdateEntryList(RaffleEntry entry)
        {
            // TODO: Replace Console.WriteLine with ILogger
            Console.WriteLine($"{DateTime.Now}: Adding {entry.MessageSid} to the entry list");
            await Task.Run(() => EntryList.Add(entry));
            Console.WriteLine($"{DateTime.Now}: Adding {entry.MessageSid} to the entry list");
            ToggleEnabledButtons(false, true, true);
            StateHasChanged();
        }

        protected async Task StartRaffle()
        {
            Console.WriteLine($"{DateTime.Now}: Starting the Raffle");
            Modal.Show("Confirm Raffle Start", typeof(raffler.Pages.StartRaffleConfirmation));
            EntryList.Clear();
            Console.WriteLine($"{DateTime.Now}: Started the Raffle");
            //ToggleEnabledButtons(false, true, true);
            StateHasChanged();
            await Task.CompletedTask;
        }

        protected async Task StopRaffle()
        {
            Console.WriteLine($"{DateTime.Now}: Stopping the Raffle");
            await RaffleService.StopRaffle();
            Console.WriteLine($"{DateTime.Now}: Stopped the Raffle");
            ToggleEnabledButtons(true, false, true);
            StateHasChanged();
        }

        protected async Task ClearRaffles()
        {
            await RaffleService.ClearRaffles();
        }

        protected async Task SelectRaffleWinner()
        {
            Console.WriteLine($"{DateTime.Now}: Selecting a raffle winner");
            var result = await RaffleService.SelectRaffleWinner();
            Console.WriteLine($"{DateTime.Now}: Resulting SID from notifying winner - {result}");
            Console.WriteLine($"{DateTime.Now}: Selected a raffle winner");
            ToggleEnabledButtons(true, false, false);
            StateHasChanged();
        }

        protected Task ShowAddPrizeModal()
        {
            Modal.Show("Add a New Prize", typeof(raffler.Pages.Prize));
            return Task.CompletedTask;
        }

        private void ToggleEnabledButtons(bool enableStart, bool enableStop, bool enableWinner)
        {
            _isStartButtonEnabled = enableStart;
            _isStopButtonEnabled = enableStop;
            _isWinnerButtonEnabled = enableWinner;
        }

        private async Task InitalizeSignalRAsync()
        {          
            // in Component Initialization code
            var connection = HubConnectionBuilder // the injected one from above.
                    .WithUrl($"{NavigationManager.BaseUri}rafflehub") // The hub URL. If the Hub is hosted on the server where the blazor is hosted, you can just use the relative path.
                    .Build(); // Build the HubConnection
            connection.On<RaffleEntry>("AddRaffleEntry", UpdateEntryList);
            await connection.StartAsync();
        }
    }
}
