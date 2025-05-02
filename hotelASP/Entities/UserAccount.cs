using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]  
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }
        public string? Password { get; set; }
        public DateOnly CreateDate { get; set; }
        [Required]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

    }
}
