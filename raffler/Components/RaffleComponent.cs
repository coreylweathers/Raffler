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
        [Inject]
        protected IStorageService StorageService { get; set; }
        [Inject]
        protected HubConnectionBuilder HubConnectionBuilder { get; set; }
        [Inject]
        protected IUriHelper UriHelper { get; set; }

        public readonly string RaffleNumber = "(425) 250-9682";
        public List<RaffleEntry> EntryList { get; set; }

        protected override async Task OnInitAsync()
        {
            await InitalizeSignalRAsync();
            EntryList = (await StorageService.GetRaffleEntriesAsync()).ToList() ?? new List<RaffleEntry>(); 
        }

        protected async Task UpdateEntryListAsync(RaffleEntry entry)
        {
            // TODO: Replace Console.WriteLine with ILogger
            Console.WriteLine($"{DateTime.Now}: Adding {entry.MessageSid} to the entry list");
            await Task.Run(() => EntryList.Add(entry));
            Console.WriteLine($"{DateTime.Now}: Adding {entry.MessageSid} to the entry list");
            StateHasChanged();
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

        protected async Task StartRaffleAsync()
        {
            Console.WriteLine($"{DateTime.Now}: Starting the Raffle");
            await StorageService.StartRaffleAsync();
            EntryList.Clear();
            Console.WriteLine($"{DateTime.Now}: Started the Raffle");
            StateHasChanged();
        }

        protected async Task StopRaffleAsync()
        {
            Console.WriteLine($"{DateTime.Now}: Stopping the Raffle");
            await StorageService.StopRaffleAsync();
            Console.WriteLine($"{DateTime.Now}: Stopped the Raffle");
        }

        protected async Task ClearRafflesAsync()
        {
            await StorageService.ClearRafflesAsync();
        }

        protected async Task SelectRaffleWinnerAsync()
        {
            Console.WriteLine($"{DateTime.Now}: Selecting a raffle winner");
            var result = await StorageService.SelectRaffleWinnerAsync();
            Console.WriteLine($"{DateTime.Now}: Resulting SID from notifying winner - {result}");
            Console.WriteLine($"{DateTime.Now}: Selected a raffle winner");
            StateHasChanged();
        }
    }
}
