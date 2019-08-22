using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace shared.Models
{
    public class Raffle
    {
        public string Name { get; set; }
        public bool Current { get; set; } = true;
        public List<RaffleEntry> Entries { get; set; } = new List<RaffleEntry>();
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public string Sid { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RaffleState State { get; set; } = RaffleState.NotRunning;

        public RafflePrize Prize { get; set; }
    }
}
