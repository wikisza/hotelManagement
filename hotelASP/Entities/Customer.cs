using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastVisitDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Relacje
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}