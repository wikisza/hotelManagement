using hotelASP.Models;
using hotelASP.Entities;

namespace hotelASP.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetActiveOrdersAsync();
        Task<List<OrderViewModel>> GetOrderHistoryAsync();
        Task<ManageMenuViewModel> GetManageMenuViewModelAsync();
        Task<Reservation> FindActiveReservationAsync(int roomNumber, string guestLastName);
        Task<Order> CreateOrderAsync(CreateOrderViewModel model); Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task AddMenuItemAsync(MenuItem menuItem);
        Task AddMenuCategoryAsync(MenuCategory menuCategory);
    }
}