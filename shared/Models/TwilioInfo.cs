using System;
using System.Collections.Generic;
using System.Text;

namespace shared.Models
{
    public class TwilioInfo
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Dictionary<string,string> SyncServiceData { get; set; }
    }
}
