using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    /// <summary>
    /// Relacja wiele-do-wielu miêdzy rolami a uprawnieniami
    /// </summary>
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;

        public int PermissionId { get; set; }
        public virtual Permission Permission { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}