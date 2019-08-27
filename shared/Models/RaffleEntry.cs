using Newtonsoft.Json;
using System;

namespace shared.Models
{
    public class RaffleEntry
    {
        [JsonIgnore]
        public string PhoneNumber { get; set; }
        public string MessageSid { get; set; }
        public DateTime TimeStamp { get; } = DateTime.UtcNow;
        [JsonProperty(Required= Required.Default)]
        public bool IsWinner { get; set; } = false;
    }
}
