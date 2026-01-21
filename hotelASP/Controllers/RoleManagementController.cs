using hotelASP.Authorization;
using hotelASP.Interfaces;
using hotelASP.Models.RoleManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Controllers
{
    [Authorize]
    public class RoleManagementController : Controller
    {
        private readonly IRoleManagementService _roleService;

        public RoleManagementController(IRoleManagementService roleService)
        {
            _roleService = roleService;
        }

        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Index()
        {
            var model = await _roleService.GetRoleListAsync();
            return View(model);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Create()
        {
            var permissions = await _roleService.GetAllPermissionsAsync();
            var model = new RoleViewModel
            {
                AvailablePermissions = permissions
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleService.CreateRoleAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "Rola zosta³a utworzona pomyœlnie!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Wyst¹pi³ b³¹d podczas tworzenia roli.");
            }

            model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();
            return View(model);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _roleService.GetRoleByIdAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Nie znaleziono roli.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Edit(RoleViewModel model, List<int> SelectedPermissionIds)
        {
            // DEBUGGING - sprawdŸ co przychodzi z formularza
            Console.WriteLine($"=== EDIT POST CALLED ===");
            Console.WriteLine($"Role ID: {model.Id}");
            Console.WriteLine($"Role Name: {model.Name}");
            Console.WriteLine($"Is System Role: {model.IsSystemRole}");
            Console.WriteLine($"SelectedPermissionIds from model: {model.SelectedPermissionIds?.Count ?? 0}");
            Console.WriteLine($"SelectedPermissionIds from parameter: {SelectedPermissionIds?.Count ?? 0}");
            
            if (SelectedPermissionIds != null && SelectedPermissionIds.Any())
            {
                Console.WriteLine($"Permission IDs: {string.Join(", ", SelectedPermissionIds)}");
                model.SelectedPermissionIds = SelectedPermissionIds;
            }
            else
            {
                Console.WriteLine("WARNING: No permission IDs received!");
                model.SelectedPermissionIds = new List<int>();
            }

            // SprawdŸ ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is INVALID:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        foreach (var error in state.Errors)
                        {
                            Console.WriteLine($"  {key}: {error.ErrorMessage}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ModelState is VALID");
            }

            // Usuñ b³êdy zwi¹zane z AvailablePermissions z ModelState
            ModelState.Remove("AvailablePermissions");

            if (ModelState.IsValid)
            {
                // Aktualizuj podstawowe informacje roli (tylko dla ról niebêd¹cych systemowymi)
                if (!model.IsSystemRole)
                {
                    Console.WriteLine("Updating role basic info...");
                    var updateResult = await _roleService.UpdateRoleAsync(model);
                    if (!updateResult)
                    {
                        Console.WriteLine("Failed to update role basic info");
                        ModelState.AddModelError("", "Nie mo¿na edytowaæ roli lub wyst¹pi³ b³¹d.");
                        model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();
                        return View(model);
                    }
                    Console.WriteLine("Role basic info updated successfully");
                }

                // Aktualizuj uprawnienia (zawsze, nawet dla ról systemowych)
                Console.WriteLine($"Updating permissions... Count: {model.SelectedPermissionIds.Count}");
                var permissionsResult = await _roleService.UpdateRolePermissionsAsync(
                    model.Id, 
                    model.SelectedPermissionIds
                );

                if (permissionsResult)
                {
                    Console.WriteLine("Permissions updated successfully!");
                    TempData["SuccessMessage"] = "Rola i uprawnienia zosta³y zaktualizowane pomyœlnie!";
                    return RedirectToAction(nameof(Index));
                }
                
                Console.WriteLine("Failed to update permissions");
                TempData["ErrorMessage"] = "Wyst¹pi³ problem z aktualizacj¹ uprawnieñ.";
            }

            Console.WriteLine("Reloading permissions for view...");
            model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();
            
            // Zaznacz checkboxy na podstawie SelectedPermissionIds
            foreach (var perm in model.AvailablePermissions)
            {
                perm.IsSelected = model.SelectedPermissionIds.Contains(perm.Id);
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Rola zosta³a usuniêta!";
            }
            else
            {
                TempData["ErrorMessage"] = "Nie mo¿na usun¹æ roli systemowej lub wyst¹pi³ b³¹d.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}