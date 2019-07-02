using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Hubs
{
    public class RaffleHub : Hub
    {
        public async Task AddNewRaffleEntry(string messageId, DateTime timestamp)
        {
            await Clients.All.SendAsync("addNewRaffleEntry", messageId, timestamp);
        }
    }
}
