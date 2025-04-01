using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [MaxLength(50, ErrorMessage = "Dozwolone maksymalnie 50 znaków.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [MaxLength(50, ErrorMessage = "Dozowolone maksymalnie 50 znaków.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [MaxLength(100, ErrorMessage = "Dozowolone maksymalnie 100 znaków.")]
        //[EmailAddress(ErrorMessage = "Proszę podać poprawny email.")]
        [RegularExpression(@"^([\w-\.]+)@((\[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Proszę podać poprawny email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username jest wymagany.")]
        [StringLength(20, MinimumLength = 5,ErrorMessage = "Dozwolone minimum 5 oraz maksymalnie 20 znaków.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Sprawdź swoje hasło.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Date)]
        public DateOnly CreateDate { get; set; }

        [Required(ErrorMessage ="Wymagane jest stanowisko pracy.")]
        public int RoleId { get; set; }
    }
}
