using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ReservationId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string SpecialRequests { get; set; }
        public Reservation? Reservation { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
}
