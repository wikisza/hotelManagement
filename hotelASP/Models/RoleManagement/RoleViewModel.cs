using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models.RoleManagement
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa roli jest wymagana")]
        [StringLength(100, ErrorMessage = "Nazwa roli nie mo¿e przekraczaæ 100 znaków")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Opis nie mo¿e przekraczaæ 500 znaków")]
        public string? Description { get; set; }

        public bool IsSystemRole { get; set; }

        // WA¯NE: Zainicjalizuj jako pust¹ listê zamiast null
        public List<int> SelectedPermissionIds { get; set; } = new List<int>();

        public List<PermissionViewModel> AvailablePermissions { get; set; } = new List<PermissionViewModel>();
    }

    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class RoleListViewModel
    {
        public List<RoleWithPermissionsViewModel> Roles { get; set; } = new();
        public Dictionary<string, List<PermissionViewModel>> PermissionsByCategory { get; set; } = new();
    }

    public class RoleWithPermissionsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public int PermissionCount { get; set; }
        public List<string> PermissionNames { get; set; } = new();
    }
}