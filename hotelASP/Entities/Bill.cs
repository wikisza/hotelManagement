using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class Bill
    {
        [Key]
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public DateTime BillDate { get; set; }
        [Precision(10,2)]
        public decimal Amount { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
