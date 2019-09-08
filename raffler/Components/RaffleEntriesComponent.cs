using Microsoft.AspNetCore.Components;
using shared.Models;
using shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class RaffleEntriesComponent : ComponentBase
    {
        [Inject] private IRaffleService RaffleService { get; set; }
        protected IList<RaffleEntry> RaffleEntries { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Task.CompletedTask;
        }
    }
}
