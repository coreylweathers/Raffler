using Microsoft.AspNetCore.Components;
using shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raffler.Components
{
    public class SettingsComponent : ComponentBase
    {
        protected SettingsModel SettingsModel { get; set; }
        protected async Task HandleValidSubmitAsync()
        {
            await Task.Run(()=> throw new NotImplementedException());
        }
    }
}
