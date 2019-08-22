using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using shared.Services;

namespace raffler.Components
{
    public class SettingsComponent : ComponentBase
    {
        [Inject]
        private IPrizeService PrizeService { get; set; }
    }
}
