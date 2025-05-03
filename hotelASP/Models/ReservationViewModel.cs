using hotelASP.Entities;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
    public class ReservationViewModel
    {
        public int Id_reservation { get; set; }
        public DateTime Date_from { get; set; }
        public DateTime Date_to { get; set; }
        public required string First_name { get; set; }
        public required string Last_name { get; set; }
        public int IdRoom { get; set; }
        public string KeyCode { get; set; }
    }
}
