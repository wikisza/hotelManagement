using hotelASP.Models.RoleManagement;

namespace hotelASP.Interfaces
{
    public interface IRoleManagementService
    {
        Task<RoleListViewModel> GetRoleListAsync();
        Task<RoleViewModel> GetRoleByIdAsync(int roleId);
        Task<bool> CreateRoleAsync(RoleViewModel model);
        Task<bool> UpdateRoleAsync(RoleViewModel model);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<List<PermissionViewModel>> GetAllPermissionsAsync();
        Task<bool> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds);
    }
}