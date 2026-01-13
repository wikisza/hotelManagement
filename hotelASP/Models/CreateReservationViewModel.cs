using hotelASP.Entities;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
    public class CreateReservationViewModel
    {
        // Krok 1: Wybór dat
        [Required(ErrorMessage = "Data rozpoczêcia jest wymagana")]
        [Display(Name = "Data rozpoczêcia")]
        public DateTime DateFrom { get; set; }

        [Required(ErrorMessage = "Data zakoñczenia jest wymagana")]
        [Display(Name = "Data zakoñczenia")]
        public DateTime DateTo { get; set; }

        // Krok 2: Dostêpne pokoje
        public List<RoomAvailabilityViewModel>? AvailableRooms { get; set; }
        public int? SelectedRoomId { get; set; }

        // Krok 3: Dane klienta
        public int? ExistingCustomerId { get; set; }
        public bool IsNewCustomer { get; set; }

        // Dane nowego klienta
        [Required(ErrorMessage = "Imiê jest wymagane")]
        [Display(Name = "Imiê")]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [Display(Name = "Nazwisko")]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawid³owy adres email")]
        [Display(Name = "Email")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [Phone(ErrorMessage = "Nieprawid³owy numer telefonu")]
        [Display(Name = "Telefon")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Adres")]
        [MaxLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Miasto")]
        [MaxLength(100)]
        public string? City { get; set; }

        [Display(Name = "Kod pocztowy")]
        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [Display(Name = "Kraj")]
        [MaxLength(100)]
        public string? Country { get; set; }

        [Display(Name = "Notatki")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Dane rezerwacji
        [Display(Name = "Kod dostêpu")]
        public string? KeyCode { get; set; }

        // Lista istniej¹cych klientów
        public List<CustomerViewModel>? ExistingCustomers { get; set; }
    }

    public class RoomAvailabilityViewModel
    {
        public int IdRoom { get; set; }
        public int RoomNumber { get; set; }
        public string? StandardName { get; set; }
        public int PeopleNumber { get; set; }
        public int BedNumber { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public int NumberOfNights { get; set; }
        public List<string>? RoomOptions { get; set; }
    }
}