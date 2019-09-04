using System.ComponentModel.DataAnnotations;

namespace shared.Models
{
    public class RafflePrize
    {
        [Required]
        [StringLength(256, ErrorMessage ="Your string does not meet the maximum length requirements")]
        public string Name { get; set; }

        [Required]
        [Display(Name ="Link to Image URL")]
        public string ImageUrl { get; set; }

        [Editable(false)]
        [Range(Constants.QUANTITY_MINIMUM, Constants.QUANTITY_MAXIMUM, ErrorMessage ="You would need to specify a number between 1 and 10")]
        public int Quantity { get; set; }
        
        [Editable(false)]
        [Display(Name ="Selected Prize")]
        public bool IsSelectedPrize { get; set; }
    }
}
