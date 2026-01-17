using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models.RoleManagement
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa roli jest wymagana")]
        [MaxLength(50, ErrorMessage = "Nazwa mo¿e mieæ maksymalnie 50 znaków")]
        [Display(Name = "Nazwa roli")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Opis mo¿e mieæ maksymalnie 200 znaków")]
        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Display(Name = "Rola systemowa")]
        public bool IsSystemRole { get; set; }

        public List<int> SelectedPermissionIds { get; set; } = new();
        
        public List<PermissionViewModel> AvailablePermissions { get; set; } = new();
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