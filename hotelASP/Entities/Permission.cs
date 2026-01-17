using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    /// <summary>
    /// Uprawnienie w systemie
    /// </summary>
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty; // Unikalny kod uprawnienia

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; } = string.Empty; // np. "Menu", "Orders", "Users"

        public bool IsSystemPermission { get; set; } // Czy uprawnienie jest systemowe

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relacje
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}