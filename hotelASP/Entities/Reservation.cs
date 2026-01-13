using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class Reservation
    {
        [Key]
        public int Id_reservation { get; set; }

        [Required]
        public DateTime Date_from { get; set; }

        [Required]
        public DateTime Date_to { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int IdRoom { get; set; }

        [MaxLength(255)]
        public string? KeyCode { get; set; }

        // Relacje
        public Customer? Customer { get; set; }
        public Room? Room { get; set; }
        public Bill? Bills { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
