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
        Task<Order> CreateOrderAsync(CreateOrderViewModel model);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task AddMenuItemAsync(MenuItem menuItem);
        Task AddMenuCategoryAsync(MenuCategory menuCategory);
        
        // Nowe metody dla edycji i usuwania
        Task<MenuItem?> GetMenuItemByIdAsync(int id);
        Task<MenuCategory?> GetMenuCategoryByIdAsync(int id);
        Task UpdateMenuItemAsync(MenuItem menuItem);
        Task UpdateMenuCategoryAsync(MenuCategory menuCategory);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<bool> DeleteMenuCategoryAsync(int id);
    }
}