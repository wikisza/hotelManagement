using hotelASP.Entities;
using hotelASP.Models;

namespace hotelASP.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerViewModel>> GetAllCustomersAsync();
        Task<CustomerDetailsViewModel> GetCustomerDetailsAsync(int customerId);
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<Customer> FindOrCreateCustomerAsync(string firstName, string lastName, string email, string phoneNumber);
        Task<(bool Success, string ErrorMessage)> CreateCustomerAsync(Customer customer);
        Task<(bool Success, string ErrorMessage)> UpdateCustomerAsync(Customer customer);
        Task<(bool Success, string ErrorMessage)> DeleteCustomerAsync(int customerId);
    }
}