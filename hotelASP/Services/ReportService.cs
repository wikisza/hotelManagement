using hotelASP.Data;
using hotelASP.Models.Reports;
using hotelASP.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncomeReportViewModel> GenerateIncomeReportAsync(DateTime dateFrom, DateTime dateTo)
        {
            var reservations = await _context.Reservations
                .Include(r => r.Bills)
                .Include(r => r.Room)
                    .ThenInclude(room => room.RoomType)
                .Where(r => r.Date_from >= dateFrom && r.Date_from <= dateTo)
                .ToListAsync();

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo)
                .ToListAsync();

            var reservationIncome = reservations
                .Where(r => r.Bills != null)
                .Sum(r => r.Bills.Amount);

            var ordersIncome = orders.Sum(o => o.TotalAmount);

            // Podzia³ miesiêczny
            var monthlyData = reservations
                .GroupBy(r => new { r.Date_from.Year, r.Date_from.Month })
                .Select(g => new MonthlyIncomeData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    ReservationIncome = g.Where(r => r.Bills != null).Sum(r => r.Bills.Amount),
                    OrdersIncome = orders
                        .Where(o => o.OrderDate.Year == g.Key.Year && o.OrderDate.Month == g.Key.Month)
                        .Sum(o => o.TotalAmount),
                    TotalIncome = g.Where(r => r.Bills != null).Sum(r => r.Bills.Amount) +
                                  orders.Where(o => o.OrderDate.Year == g.Key.Year && o.OrderDate.Month == g.Key.Month)
                                        .Sum(o => o.TotalAmount)
                })
                .OrderBy(m => m.Month)
                .ToList();

            // Podzia³ wed³ug typu pokoju
            var roomTypeData = reservations
                .Where(r => r.Room?.RoomType != null)
                .GroupBy(r => r.Room.RoomType.DisplayName)
                .Select(g => new RoomTypeIncomeData
                {
                    RoomType = g.Key,
                    ReservationsCount = g.Count(),
                    Income = g.Where(r => r.Bills != null).Sum(r => r.Bills.Amount)
                })
                .OrderByDescending(r => r.Income)
                .ToList();

            return new IncomeReportViewModel
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                ReservationIncome = reservationIncome,
                OrdersIncome = ordersIncome,
                TotalIncome = reservationIncome + ordersIncome,
                TotalReservations = reservations.Count,
                TotalOrders = orders.Count,
                MonthlyBreakdown = monthlyData,
                RoomTypeBreakdown = roomTypeData
            };
        }

        public async Task<CustomerReportViewModel> GenerateCustomerReportAsync(DateTime dateFrom, DateTime dateTo)
        {
            var customers = await _context.Customers
                .Include(c => c.Reservations)
                    .ThenInclude(r => r.Bills)
                .Where(c => c.Reservations.Any(r => r.Date_from >= dateFrom && r.Date_from <= dateTo))
                .ToListAsync();

            var newCustomers = customers
                .Where(c => c.CreatedDate >= dateFrom && c.CreatedDate <= dateTo)
                .Count();

            var returningCustomers = customers.Count - newCustomers;

            // Top klienci
            var topCustomers = customers
                .Select(c => new CustomerDetailData
                {
                    CustomerName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email,
                    ReservationsCount = c.Reservations
                        .Count(r => r.Date_from >= dateFrom && r.Date_from <= dateTo),
                    TotalSpent = c.Reservations
                        .Where(r => r.Date_from >= dateFrom && r.Date_from <= dateTo && r.Bills != null)
                        .Sum(r => r.Bills.Amount),
                    LastVisit = c.LastVisitDate ?? c.CreatedDate
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            // Miesiêczny podzia³
            var monthlyData = await _context.Reservations
                .Where(r => r.Date_from >= dateFrom && r.Date_from <= dateTo)
                .GroupBy(r => new { r.Date_from.Year, r.Date_from.Month })
                .Select(g => new MonthlyCustomerData
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    NewCustomers = _context.Customers
                        .Count(c => c.CreatedDate.Year == g.Key.Year && 
                                   c.CreatedDate.Month == g.Key.Month &&
                                   c.CreatedDate >= dateFrom && 
                                   c.CreatedDate <= dateTo),
                    TotalVisits = g.Count()
                })
                .OrderBy(m => m.Month)
                .ToListAsync();

            return new CustomerReportViewModel
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalCustomers = customers.Count,
                NewCustomers = newCustomers,
                ReturningCustomers = returningCustomers,
                TopCustomers = topCustomers,
                MonthlyBreakdown = monthlyData
            };
        }

        public async Task<OrderReportViewModel> GenerateOrderReportAsync(DateTime dateFrom, DateTime dateTo)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                .Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo)
                .ToListAsync();

            var totalValue = orders.Sum(o => o.TotalAmount);
            var averageValue = orders.Any() ? totalValue / orders.Count : 0;

            // Zamówienia wed³ug statusu
            var ordersByStatus = orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusData
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            // Popularne pozycje menu
            var popularItems = orders
                .SelectMany(o => o.OrderDetails)
                .Where(od => od.MenuItem != null)
                .GroupBy(od => od.MenuItem.Name)
                .Select(g => new PopularMenuItemData
                {
                    ItemName = g.Key,
                    OrderCount = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Quantity * od.Price)
                })
                .OrderByDescending(p => p.OrderCount)
                .Take(10)
                .ToList();

            // Dzienny podzia³
            var dailyData = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailyOrderData
                {
                    Date = g.Key,
                    OrdersCount = g.Count(),
                    TotalValue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            return new OrderReportViewModel
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                TotalOrders = orders.Count,
                TotalOrdersValue = totalValue,
                AverageOrderValue = averageValue,
                OrdersByStatus = ordersByStatus,
                PopularItems = popularItems,
                DailyBreakdown = dailyData
            };
        }

        // Metody importu - placeholder (opcjonalnie do implementacji)
        public Task ImportIncomeReportAsync(Stream stream)
        {
            throw new NotImplementedException("Import funkcjonalnoœæ nie jest jeszcze zaimplementowana.");
        }

        public Task ImportCustomerReportAsync(Stream stream)
        {
            throw new NotImplementedException("Import funkcjonalnoœæ nie jest jeszcze zaimplementowana.");
        }

        public Task ImportOrderReportAsync(Stream stream)
        {
            throw new NotImplementedException("Import funkcjonalnoœæ nie jest jeszcze zaimplementowana.");
        }
    }
}