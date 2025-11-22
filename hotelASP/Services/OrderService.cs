using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderViewModel>> GetActiveOrdersAsync()
        {
            var activeStatuses = new[] { "Nowe", "W przygotowaniu", "Gotowe do dostarczenia" };
            return await _context.Orders
                .Where(o => activeStatuses.Contains(o.Status))
                .Include(o => o.Reservation)
                    .ThenInclude(r => r.Room)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderDateTime = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    RoomNumber = o.Reservation.Room.RoomNumber,
                    Details = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        MenuItemName = od.MenuItem.Name,
                        Quantity = od.Quantity,
                        PriceAtOrder = od.Price
                    }).ToList()
                })
                .OrderBy(o => o.OrderDateTime)
                .ToListAsync();
        }

        public async Task<List<OrderViewModel>> GetOrderHistoryAsync()
        {
            var inactiveStatuses = new[] { "Dostarczone", "Anulowane" };
            return await _context.Orders
                .Where(o => inactiveStatuses.Contains(o.Status))
                .Include(o => o.Reservation)
                    .ThenInclude(r => r.Room)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderDateTime = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    RoomNumber = o.Reservation.Room.RoomNumber
                })
                .ToListAsync();
        }

        public async Task<ManageMenuViewModel> GetManageMenuViewModelAsync()
        {
            return new ManageMenuViewModel
            {
                Categories = await _context.MenuCategories.ToListAsync(),
                MenuItems = await _context.MenuItems.Include(mi => mi.MenuCategory).ToListAsync(),
                NewMenuItem = new MenuItem()
            };
        }

        public async Task<Reservation> FindActiveReservationAsync(int roomNumber, string guestLastName)
        {
            var now = DateTime.Now;
            var guestLastNameLower = guestLastName.ToLower();
            return await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r =>
                    r.Room.RoomNumber == roomNumber &&
                    r.Last_name.ToLower() == guestLastNameLower &&
                    r.Date_from <= now &&
                    r.Date_to >= now);
        }

        public async Task<Order> CreateOrderAsync(CreateOrderViewModel model)
        {
            if (model.ReservationId == null || !model.SelectedItems.Any(i => i.Quantity > 0))
            {
                return null;
            }

            var itemsToOrder = model.SelectedItems.Where(i => i.Quantity > 0).ToList();
            var itemIds = itemsToOrder.Select(i => i.MenuItemId).ToList();
            var menuItems = await _context.MenuItems
                                          .Where(mi => itemIds.Contains(mi.Id))
                                          .ToDictionaryAsync(mi => mi.Id, mi => mi);

            var newOrder = new Order
            {
                ReservationId = model.ReservationId.Value,
                OrderDate = DateTime.Now,
                Status = "Nowe",
                OrderDetails = new List<OrderDetails>()
            };

            decimal totalAmount = 0;

            foreach (var item in itemsToOrder)
            {
                if (menuItems.TryGetValue(item.MenuItemId, out var menuItem))
                {
                    var orderDetail = new OrderDetails
                    {
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = menuItem.Price
                    };
                    newOrder.OrderDetails.Add(orderDetail);
                    totalAmount += menuItem.Price * item.Quantity;
                }
            }

            newOrder.TotalAmount = totalAmount;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            if (totalAmount > 0)
            {
                newOrder.TotalAmount = totalAmount;

                _context.Orders.Add(newOrder);

                var bill = await _context.Bills
                                         .FirstOrDefaultAsync(b => b.ReservationId == model.ReservationId.Value);

                if (bill == null)
                {
                    bill = new Bill
                    {
                        ReservationId = model.ReservationId.Value,
                        Amount = 0,
                        Status = "Nieopłacone"
                    };
                    _context.Bills.Add(bill);
                }

                bill.Amount += newOrder.TotalAmount;

                await _context.SaveChangesAsync();
            }

            return newOrder;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddMenuItemAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task AddMenuCategoryAsync(MenuCategory menuCategory)
        {
            _context.MenuCategories.Add(menuCategory);
            await _context.SaveChangesAsync();
        }
    }
}