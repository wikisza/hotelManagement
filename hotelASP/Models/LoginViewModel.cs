using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username lub email jest wymagany.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Dozwolone minimum 5 oraz maksymalnie 50 znaków.")]
        [DisplayName("Username lub email")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


    }


}
