using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    /// <summary>
    /// Rola użytkownika w systemie
    /// </summary>
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsSystemRole { get; set; } // Czy rola jest systemowa (nie można usunąć)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Relacje
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserAccount> Users { get; set; } = new List<UserAccount>();
    }
}
