using Microsoft.AspNetCore.SignalR;
using shared.Models;
using System.Threading.Tasks;

namespace raffler.Hubs
{
    public class RaffleHub : Hub
    {
        public async Task AddRaffleEntry(RaffleEntry entry)
        {
            await Clients.All.SendAsync("AddRaffleEntry", entry);
        }
    }
}
