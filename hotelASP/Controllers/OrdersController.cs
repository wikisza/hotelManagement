using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<IActionResult> Index()
        {
            var activeOrders = await _orderService.GetActiveOrdersAsync();
            return View(activeOrders);
        }

        public async Task<IActionResult> History()
        {
            var orderHistory = await _orderService.GetOrderHistoryAsync();
            return View(orderHistory);
        }

        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> ManageMenu()
        {
            var model = await _orderService.GetManageMenuViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenuItem([Bind(Prefix = "NewMenuItem")] MenuItem menuItem)
        {
            if (ModelState.IsValid)
            {
                await _orderService.AddMenuItemAsync(menuItem);
                return RedirectToAction(nameof(ManageMenu));
            }
            var viewModel = await _orderService.GetManageMenuViewModelAsync();
            viewModel.NewMenuItem = menuItem;
            return View("ManageMenu", viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMenuCategory(ManageMenuViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.NewCategory.Name))
            {
                await _orderService.AddMenuCategoryAsync(model.NewCategory);
                return RedirectToAction(nameof(ManageMenu));
            }

            TempData["CategoryError"] = "Nazwa kategorii nie może być pusta.";
            var viewModel = await _orderService.GetManageMenuViewModelAsync();
            return View("ManageMenu", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "manager,admin,employee")]
        public IActionResult Create()
        {
            return PartialView("_CreateOrderStep1", new CreateOrderViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "manager,admin,employee")]
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
                GuestFullName = $"{reservation.First_name} {reservation.Last_name}",
                AvailableMenuItems = await _context.MenuCategories
                    .Include(c => c.MenuItems.Where(mi => mi.IsAvailable))
                    .OrderBy(c => c.Name)
                    .ToListAsync()
            };

            return PartialView("_CreateOrderStep2", orderModel);
        }

        [HttpPost]
        [Authorize(Roles = "manager,admin,employee")]
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