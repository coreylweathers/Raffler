namespace shared.Models
{
    public class RafflePrize
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public int Quantity { get; set; }
        
        public bool IsSelectedPrize { get; set; }
    }
}
