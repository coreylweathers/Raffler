using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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

        public RaffleState State { get; set; } = RaffleState.NotRunning;
    }
}
