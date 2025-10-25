namespace hotelASP.Models
{
    public class OrderDetailViewModel
    {
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
    }
}