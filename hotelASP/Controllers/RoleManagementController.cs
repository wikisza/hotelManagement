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
            try
            {
                var model = await _roleService.GetRoleByIdAsync(id);
                if (model == null)
                {
                    TempData["ErrorMessage"] = "Nie znaleziono roli.";
                    return RedirectToAction(nameof(Index));
                }

                // DEBUGGING - sprawdŸ w konsoli czy dane s¹ ³adowane
                Console.WriteLine($"Editing role: {model.Name}, Permissions count: {model.AvailablePermissions?.Count ?? 0}");
                
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"B³¹d podczas ³adowania roli: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Aktualizuj podstawowe informacje roli
                var updateResult = await _roleService.UpdateRoleAsync(model);
                if (!updateResult)
                {
                    ModelState.AddModelError("", "Nie mo¿na edytowaæ roli systemowej lub wyst¹pi³ b³¹d.");
                    model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();
                    return View(model);
                }

                // Aktualizuj uprawnienia
                var permissionsResult = await _roleService.UpdateRolePermissionsAsync(
                    model.Id, 
                    model.SelectedPermissionIds ?? new List<int>()
                );

                if (permissionsResult)
                {
                    TempData["SuccessMessage"] = "Rola i uprawnienia zosta³y zaktualizowane!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Rola zosta³a zaktualizowana, ale wyst¹pi³ problem z aktualizacj¹ uprawnieñ.";
                    return RedirectToAction(nameof(Index));
                }
            }

            model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(PermissionCodes.RoleManage)]
        public async Task<IActionResult> UpdatePermissions(int roleId, [FromBody] List<int> permissionIds)
        {
            var result = await _roleService.UpdateRolePermissionsAsync(roleId, permissionIds);
            if (result)
            {
                return Json(new { success = true, message = "Uprawnienia zosta³y zaktualizowane!" });
            }

            return Json(new { success = false, message = "Wyst¹pi³ b³¹d podczas aktualizacji uprawnieñ." });
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