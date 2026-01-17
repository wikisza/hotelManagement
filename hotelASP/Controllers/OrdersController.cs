using hotelASP.Authorization;
using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext _context;

        public OrdersController(IOrderService orderService, ApplicationDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HasPermission(PermissionCodes.OrderView)]
        public async Task<IActionResult> Index()
        {
            var activeOrders = await _orderService.GetActiveOrdersAsync();
            return View(activeOrders);
        }

        [HasPermission(PermissionCodes.OrderHistory)]
        public async Task<IActionResult> History()
        {
            var orderHistory = await _orderService.GetOrderHistoryAsync();
            return View(orderHistory);
        }

        [HasPermission(PermissionCodes.MenuManage)]
        public async Task<IActionResult> ManageMenu()
        {
            var model = await _orderService.GetManageMenuViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.MenuItemAdd)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenuItem([Bind(Prefix = "NewMenuItem")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                await _orderService.AddMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Pozycja menu została dodana pomyślnie!";
                return RedirectToAction(nameof(ManageMenu));
            }
            var viewModel = await _orderService.GetManageMenuViewModelAsync();
            viewModel.NewMenuItem = menuItem;
            return View("ManageMenu", viewModel);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.MenuCategoryAdd)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenuCategory(ManageMenuViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewCategory.Name))
            {
                await _orderService.AddMenuCategoryAsync(model.NewCategory);
                TempData["SuccessMessage"] = "Kategoria została dodana pomyślnie!";
                return RedirectToAction(nameof(ManageMenu));
            }

            TempData["CategoryError"] = "Nazwa kategorii nie może być pusta.";
            var viewModel = await _orderService.GetManageMenuViewModelAsync();
            return View("ManageMenu", viewModel);
        }

        [HttpGet]
        [HasPermission(PermissionCodes.MenuCategoryEdit)]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _orderService.GetMenuCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_EditCategoryModal", category);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.MenuCategoryEdit)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(MenuCategory category)
        {
            if (ModelState.IsValid)
            {
                await _orderService.UpdateMenuCategoryAsync(category);
                TempData["SuccessMessage"] = "Kategoria została zaktualizowana!";
                return RedirectToAction(nameof(ManageMenu));
            }
            return RedirectToAction(nameof(ManageMenu));
        }

        [HttpPost]
        [HasPermission(PermissionCodes.MenuCategoryDelete)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _orderService.DeleteMenuCategoryAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Kategoria została usunięta!";
            }
            else
            {
                TempData["ErrorMessage"] = "Nie można usunąć kategorii zawierającej pozycje menu.";
            }
            return RedirectToAction(nameof(ManageMenu));
        }

        [HttpGet]
        [HasPermission(PermissionCodes.MenuItemEdit)]
        public async Task<IActionResult> EditMenuItem(int id)
        {
            var menuItem = await _orderService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            
            ViewBag.Categories = await _context.MenuCategories.ToListAsync();
            return PartialView("_EditMenuItemModal", menuItem);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.MenuItemEdit)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItem(MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                await _orderService.UpdateMenuItemAsync(menuItem);
                TempData["SuccessMessage"] = "Pozycja menu została zaktualizowana!";
                return RedirectToAction(nameof(ManageMenu));
            }
            return RedirectToAction(nameof(ManageMenu));
        }

        [HttpPost]
        [HasPermission(PermissionCodes.MenuItemDelete)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var result = await _orderService.DeleteMenuItemAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Pozycja menu została usunięta lub oznaczona jako niedostępna!";
            }
            else
            {
                TempData["ErrorMessage"] = "Nie można usunąć pozycji menu.";
            }
            return RedirectToAction(nameof(ManageMenu));
        }

        [HttpPost]
        [HasPermission(PermissionCodes.OrderUpdateStatus)]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return Ok();
        }

        [HttpGet]
        [HasPermission(PermissionCodes.OrderCreate)]
        public IActionResult Create()
        {
            return PartialView("_CreateOrderStep1", new CreateOrderViewModel());
        }

        [HttpPost]
        [HasPermission(PermissionCodes.OrderCreate)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyGuest(CreateOrderViewModel model)
        {
            if ((model.RoomNumber == 0) || string.IsNullOrEmpty(model.GuestLastName))
            {
                return Json(new { success = false, message = "Numer pokoju i nazwisko są wymagane." });
            }

            var reservation = await _orderService.FindActiveReservationAsync(model.RoomNumber, model.GuestLastName);

            if (reservation == null)
            {
                return Json(new { success = false, message = "Nie znaleziono aktywnej rezerwacji dla podanych danych." });
            }

            var orderModel = new CreateOrderViewModel
            {
                ReservationId = reservation.Id_reservation,
                RoomNumber = reservation.Room.RoomNumber,
                GuestFullName = reservation.Customer != null 
                    ? $"{reservation.Customer.FirstName} {reservation.Customer.LastName}" 
                    : "Nieznany gość",
                AvailableMenuItems = await _context.MenuCategories
                    .Include(c => c.MenuItems.Where(mi => mi.IsAvailable))
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return PartialView("_CreateOrderStep2", orderModel);
        }

        [HttpPost]
        [HasPermission(PermissionCodes.OrderCreate)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder(CreateOrderViewModel model)
        {
            if (model.SelectedItems == null || !model.SelectedItems.Any(i => i.Quantity > 0))
            {
                TempData["OrderError"] = "Zamówienie nie może być puste.";
                return RedirectToAction("Index");
            }

            await _orderService.CreateOrderAsync(model);

            TempData["SuccessMessage"] = "Nowe zamówienie zostało pomyślnie złożone!";
            return RedirectToAction("Index");
        }
    }
}