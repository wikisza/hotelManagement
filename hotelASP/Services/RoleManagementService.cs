using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models.RoleManagement;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly ApplicationDbContext _context;

        public RoleManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RoleListViewModel> GetRoleListAsync()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            var permissions = await _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var viewModel = new RoleListViewModel
            {
                Roles = roles.Select(r => new RoleWithPermissionsViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    PermissionCount = r.RolePermissions.Count,
                    PermissionNames = r.RolePermissions
                        .Select(rp => rp.Permission.Name)
                        .OrderBy(n => n)
                        .ToList()
                }).ToList(),

                PermissionsByCategory = permissions
                    .GroupBy(p => p.Category)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(p => new PermissionViewModel
                        {
                            Id = p.Id,
                            Code = p.Code,
                            Name = p.Name,
                            Description = p.Description,
                            Category = p.Category
                        }).ToList()
                    )
            };

            return viewModel;
        }

        public async Task<RoleViewModel> GetRoleByIdAsync(int roleId)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                return null!;

            var allPermissions = await _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();

            var selectedPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();

            return new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                SelectedPermissionIds = selectedPermissionIds,
                AvailablePermissions = allPermissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category,
                    IsSelected = selectedPermissionIds.Contains(p.Id)
                }).ToList()
            };
        }

        public async Task<bool> CreateRoleAsync(RoleViewModel model)
        {
            try
            {
                var role = new Role
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsSystemRole = false,
                    CreatedAt = DateTime.Now
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                // Przypisz uprawnienia
                if (model.SelectedPermissionIds.Any())
                {
                    var rolePermissions = model.SelectedPermissionIds.Select(permId => new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permId,
                        AssignedAt = DateTime.Now
                    });

                    _context.RolePermissions.AddRange(rolePermissions);
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(RoleViewModel model)
        {
            try
            {
                var role = await _context.Roles.FindAsync(model.Id);
                if (role == null || role.IsSystemRole)
                    return false;

                role.Name = model.Name;
                role.Description = model.Description;
                role.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role == null || role.IsSystemRole)
                    return false;

                // Usuñ przypisania uprawnieñ
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(rolePermissions);
                _context.Roles.Remove(role);

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PermissionViewModel>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Category = p.Category
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                    return false;

                // Usuñ wszystkie obecne uprawnienia
                _context.RolePermissions.RemoveRange(role.RolePermissions);

                // Dodaj nowe uprawnienia
                var newRolePermissions = permissionIds.Select(permId => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId,
                    AssignedAt = DateTime.Now
                });

                _context.RolePermissions.AddRange(newRolePermissions);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}