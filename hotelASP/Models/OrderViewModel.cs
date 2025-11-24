namespace hotelASP.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int RoomNumber { get; set; }
        public ICollection<OrderDetailViewModel> Details { get; set; } = new List<OrderDetailViewModel>();
    }
}