using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace raffler.Components
{
    public class RaffleComponent : ComponentBase
    {
        [Inject]
        protected IStorageService StorageService { get; set; }
        public readonly string RaffleNumber = "(425) 250-9682";
        public List<RaffleEntry> EntryList { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            EntryList = (await StorageService.GetRaffleEntriesAsync()).ToList();
        }
    }
}
