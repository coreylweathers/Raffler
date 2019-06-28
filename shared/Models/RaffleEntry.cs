using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shared.Models
{
    public class RaffleEntry
    {
        public string Digits { get; set; }
        public string MessageSid { get; set; }
        public string EmailAddress { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
