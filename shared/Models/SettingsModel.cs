using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace shared.Models
{
    public class SettingsModel
    {
        [Required, StringLength(10,ErrorMessage ="Your Raffle Name is too long")]
        public string Name { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public List<RafflePrize> Prizes { get; set; }
        
        public TwilioInfo TwilioInformation { get; set; }

    }
}
