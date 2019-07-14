using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models
{
    public class RaffleEntry
    {
        public string MessageSid { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        [JsonProperty(Required= Required.Default)]
        public bool IsWinner { get; set; } = false;
    }
}
