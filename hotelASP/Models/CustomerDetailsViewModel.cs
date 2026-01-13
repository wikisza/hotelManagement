using hotelASP.Entities;

namespace hotelASP.Models
{
    public class CustomerDetailsViewModel
    {
        public Customer Customer { get; set; }
        public List<ReservationViewModel> Reservations { get; set; } = new List<ReservationViewModel>();
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public decimal TotalSpent { get; set; }
        public int TotalNightsStayed { get; set; }
    }
}