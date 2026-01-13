using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerViewModel>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    CreatedDate = c.CreatedDate,
                    LastVisitDate = c.LastVisitDate,
                    TotalReservations = c.Reservations.Count,
                    TotalOrders = c.Reservations.SelectMany(r => r.Orders).Count()
                })
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<CustomerDetailsViewModel> GetCustomerDetailsAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Reservations)
                    .ThenInclude(r => r.Room)
                .Include(c => c.Reservations)
                    .ThenInclude(r => r.Bills)
                .Include(c => c.Reservations)
                    .ThenInclude(r => r.Orders)
                        .ThenInclude(o => o.OrderDetails)
                            .ThenInclude(od => od.MenuItem)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                return null;

            var viewModel = new CustomerDetailsViewModel
            {
                Customer = customer,
                Reservations = customer.Reservations
                    .OrderByDescending(r => r.Date_from)
                    .Select(r => new ReservationViewModel
                    {
                        Id_reservation = r.Id_reservation,
                        Date_from = r.Date_from,
                        Date_to = r.Date_to,
                        IdRoom = r.IdRoom,
                        KeyCode = r.KeyCode,
                        Room = r.Room,
                        Bill = r.Bills != null ? new BillViewModel
                        {
                            Id = r.Bills.Id,
                            Amount = r.Bills.Amount,
                            Status = r.Bills.Status,
                            BillDate = r.Bills.BillDate
                        } : null
                    })
                    .ToList(),
                Orders = customer.Reservations
                    .SelectMany(r => r.Orders)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        OrderDateTime = o.OrderDate,
                        Status = o.Status,
                        TotalAmount = o.TotalAmount,
                        Details = o.OrderDetails.Select(od => new OrderDetailViewModel
                        {
                            MenuItemName = od.MenuItem.Name,
                            Quantity = od.Quantity,
                            PriceAtOrder = od.Price
                        }).ToList()
                    })
                    .ToList(),
                TotalSpent = customer.Reservations
                    .Where(r => r.Bills != null)
                    .Sum(r => r.Bills.Amount),
                TotalNightsStayed = customer.Reservations
                    .Sum(r => (int)(r.Date_to - r.Date_from).TotalDays)
            };

            return viewModel;
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<Customer> FindOrCreateCustomerAsync(string firstName, string lastName, string email, string phoneNumber)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email || c.PhoneNumber == phoneNumber);

            if (customer == null)
            {
                customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    CreatedDate = DateTime.Now
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            else
            {
                customer.LastVisitDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return customer;
        }

        public async Task<(bool Success, string ErrorMessage)> CreateCustomerAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == customer.Email || c.PhoneNumber == customer.PhoneNumber);

                if (existingCustomer != null)
                {
                    return (false, "Klient z podanym adresem email lub numerem telefonu ju¿ istnieje.");
                }

                customer.CreatedDate = DateTime.Now;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Wyst¹pi³ b³¹d podczas tworzenia klienta: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(customer.Id);

                if (existingCustomer == null)
                {
                    return (false, "Klient nie zosta³ znaleziony.");
                }

                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.Email = customer.Email;
                existingCustomer.PhoneNumber = customer.PhoneNumber;
                existingCustomer.Address = customer.Address;
                existingCustomer.City = customer.City;
                existingCustomer.PostalCode = customer.PostalCode;
                existingCustomer.Country = customer.Country;
                existingCustomer.Notes = customer.Notes;

                await _context.SaveChangesAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Wyst¹pi³ b³¹d podczas aktualizacji klienta: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .Include(c => c.Reservations)
                    .FirstOrDefaultAsync(c => c.Id == customerId);

                if (customer == null)
                {
                    return (false, "Klient nie zosta³ znaleziony.");
                }

                if (customer.Reservations.Any())
                {
                    return (false, "Nie mo¿na usun¹æ klienta, który ma rezerwacje w systemie.");
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Wyst¹pi³ b³¹d podczas usuwania klienta: {ex.Message}");
            }
        }
    }
}