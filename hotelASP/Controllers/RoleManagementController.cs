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
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            Console.WriteLine($"=== EDIT POST CALLED ===");
            Console.WriteLine($"Role ID: {model.Id}");
            Console.WriteLine($"SelectedPermissionIds from model: {model.SelectedPermissionIds?.Count ?? 0}");

            if (model.SelectedPermissionIds == null)
            {
                model.SelectedPermissionIds = new List<int>();
            }

            ModelState.Remove("AvailablePermissions");

            if (ModelState.IsValid)
            {
                if (!model.IsSystemRole)
                {
                    var updateResult = await _roleService.UpdateRoleAsync(model);
                    if (!updateResult)
                    {
                        ModelState.AddModelError("", "Nie mo¿na edytowaæ roli lub wyst¹pi³ b³¹d.");
                        model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();
                        RehydratePermissionsCheckboxes(model); 
                        return View(model);
                    }
                }

                var permissionsResult = await _roleService.UpdateRolePermissionsAsync(
                    model.Id,
                    model.SelectedPermissionIds
                );

                if (permissionsResult)
                {
                    TempData["SuccessMessage"] = "Rola i uprawnienia zosta³y zaktualizowane pomyœlnie!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Wyst¹pi³ problem z aktualizacj¹ uprawnieñ.";
            }

            model.AvailablePermissions = await _roleService.GetAllPermissionsAsync();

            foreach (var perm in model.AvailablePermissions)
            {
                perm.IsSelected = model.SelectedPermissionIds.Contains(perm.Id);
            }

            return View(model);
        }

        private void RehydratePermissionsCheckboxes(RoleViewModel model)
        {
            foreach (var perm in model.AvailablePermissions)
            {
                perm.IsSelected = model.SelectedPermissionIds.Contains(perm.Id);
            }
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