using hotelASP.Entities;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Numer pokoju jest wymagany.")]
        [Display(Name = "Numer pokoju")]
        public int RoomNumber { get; set; }

        [Required(ErrorMessage = "Nazwisko gościa jest wymagane.")]
        [Display(Name = "Nazwisko gościa")]
        public string GuestLastName { get; set; }
        
        public int? ReservationId { get; set; }
        
        public string GuestFullName { get; set; }

        public List<MenuCategory> AvailableMenuItems { get; set; } = new List<MenuCategory>();

        public List<OrderItemInput> SelectedItems { get; set; } = new List<OrderItemInput>();
    }
}