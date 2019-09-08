using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using shared.Models;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class RaffleComponent : ComponentBase
    {
        [Inject] private IRaffleService RaffleService { get; set; }
        [Inject] private IModalService Modal { get; set; }
        [Inject] private HubConnectionBuilder HubConnectionBuilder { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected readonly string RaffleNumber = "(404) 800-3505";
        protected List<RaffleEntry> EntryList { get; set; }


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
            StateHasChanged();
        }



        protected async Task ClearRaffles()
        {
            await RaffleService.ClearRaffles();
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
