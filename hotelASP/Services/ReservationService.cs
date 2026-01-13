using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;
    private readonly IBillService _billService;
    private readonly ICustomerService _customerService;

    public ReservationService(ApplicationDbContext context, IBillService billService, ICustomerService customerService)
    {
        _context = context;
        _billService = billService;
        _customerService = customerService;
    }

    public async Task<ReservationViewModel> FindReservation(int id)
    {
        var reservation = await _context.Reservations
            .Where(r => r.Id_reservation == id)
            .Include(r => r.Customer)
            .Include(r => r.Room)
            .Include(r => r.Bills)
            .Include(r => r.Orders)
                .ThenInclude(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
            .Select(r => new ReservationViewModel
            {
                Id_reservation = r.Id_reservation,
                Date_from = r.Date_from,
                Date_to = r.Date_to,
                CustomerId = r.CustomerId,
                First_name = r.Customer.FirstName,
                Last_name = r.Customer.LastName,
                IdRoom = r.IdRoom,
                KeyCode = r.KeyCode,
                Customer = r.Customer,

                Bill = r.Bills != null ? new BillViewModel
                {
                    Id = r.Bills.Id,
                    Amount = r.Bills.Amount,
                    Status = r.Bills.Status,
                    BillDate = r.Bills.BillDate
                } : null,

                Orders = r.Orders.Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderDateTime = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    Details = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        MenuItemName = od.MenuItem.Name,
                        Quantity = od.Quantity,
                        PriceAtOrder = od.Price
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return reservation;
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(Reservation reservation)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (reservation.Date_from.TimeOfDay != new TimeSpan(14, 0, 0))
            {
                reservation.Date_from = reservation.Date_from.Date.AddHours(14);
            }

            if (reservation.Date_to.TimeOfDay != new TimeSpan(10, 0, 0))
            {
                reservation.Date_to = reservation.Date_to.Date.AddHours(10);
            }

            var overlappingReservations = await _context.Reservations
                .Where(r => r.IdRoom == reservation.IdRoom &&
                            r.Date_from < reservation.Date_to &&
                            r.Date_to > reservation.Date_from)
                .ToListAsync();

            if (overlappingReservations.Any())
            {
                return (false, "Pokój jest już zarezerwowany w tym terminie.");
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await _billService.CreateBillForReservationAsync(reservation.Id_reservation);

            var thisRoom = await _context.Rooms.FindAsync(reservation.IdRoom);
            if (thisRoom != null)
            {
                thisRoom.IsEmpty = true;
            }

            var customer = await _context.Customers.FindAsync(reservation.CustomerId);
            if (customer != null)
            {
                customer.LastVisitDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return (true, string.Empty);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return (false, "Wystąpił nieoczekiwany błąd podczas tworzenia rezerwacji i rachunku.");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> EditAsync(Reservation reservation)
    {
        try
        {
            var existingReservation = await _context.Reservations.FindAsync(reservation.Id_reservation);

            if (existingReservation == null)
            {
                return (false, "Rezerwacja nie została znaleziona.");
            }

            existingReservation.Date_from = reservation.Date_from;
            existingReservation.Date_to = reservation.Date_to;
            existingReservation.CustomerId = reservation.CustomerId;
            existingReservation.IdRoom = reservation.IdRoom;
            existingReservation.KeyCode = reservation.KeyCode;

            _context.Reservations.Update(existingReservation);
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Reservations.Any(r => r.Id_reservation == reservation.Id_reservation))
            {
                return (false, "Rezerwacja nie została znaleziona.");
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<(bool Success, string ErrorMessage)> DeleteConfirmed(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
        }
        var thisRoom = await _context.Rooms.FindAsync(reservation.IdRoom);
        if (thisRoom != null)
        {
            thisRoom.IsEmpty = false;
        }
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public List<ReservationViewModel> CurrentReservations()
    {
        var currentReservations = _context.Reservations
            .Where(r => r.Date_to >= DateTime.Now)
            .OrderBy(r => r.Date_from)
            .Include(r => r.Customer)
            .Include(r => r.Room)
            .Select(r => new ReservationViewModel
            {
                Id_reservation = r.Id_reservation,
                Date_from = r.Date_from,
                Date_to = r.Date_to,
                CustomerId = r.CustomerId,
                First_name = r.Customer.FirstName,
                Last_name = r.Customer.LastName,
                IdRoom = r.IdRoom,
                KeyCode = r.KeyCode,
                Customer = r.Customer
            })
            .ToList();
        return currentReservations;
    }

    public List<ReservationViewModel> HistoryReservations()
    {
        var historyReservations = _context.Reservations
            .Where(r => r.Date_to <= DateTime.Now)
            .OrderByDescending(r => r.Date_from)
            .Include(r => r.Customer)
            .Include(r => r.Room)
            .Select(r => new ReservationViewModel
            {
                Id_reservation = r.Id_reservation,
                Date_from = r.Date_from,
                Date_to = r.Date_to,
                CustomerId = r.CustomerId,
                First_name = r.Customer.FirstName,
                Last_name = r.Customer.LastName,
                IdRoom = r.IdRoom,
                KeyCode = r.KeyCode,
                Customer = r.Customer
            })
            .ToList();
        return historyReservations;
    }

    public async Task<List<object>> GetReservations()
    {
        var now = DateTime.Now;
        return await _context.Reservations
            .Where(r => r.Date_to > now)
            .Include(r => r.Customer)
            .Include(r => r.Room)
            .Select(r => new
            {
                start = r.Date_from,
                end = r.Date_to,
                title = "Pokój " + r.Room.RoomNumber + ": " + r.Customer.LastName,
                extendedProps = new
                {
                    firstName = r.Customer.FirstName,
                    lastName = r.Customer.LastName,
                    roomNumber = r.Room.RoomNumber,
                    reservationId = r.Id_reservation
                }
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<List<object>> GetOldReservations()
    {
        var now = DateTime.Now;
        return await _context.Reservations
            .Where(r => r.Date_to <= now)
            .Include(r => r.Customer)
            .Include(r => r.Room)
            .Select(r => new
            {
                start = r.Date_from,
                end = r.Date_to,
                title = "Pokój " + r.Room.RoomNumber + ": " + r.Customer.LastName,
                extendedProps = new
                {
                    firstName = r.Customer.FirstName,
                    lastName = r.Customer.LastName,
                    roomNumber = r.Room.RoomNumber,
                    reservationId = r.Id_reservation
                }
            })
            .Cast<object>()
            .ToListAsync();
    }

    // NOWA METODA - zwraca tylko ID pokoi
    public async Task<List<int>> GetAvailableRoomIds(DateTime dateFrom, DateTime dateTo)
    {
        return await _context.Rooms
            .Where(room => !_context.Reservations
                .Any(reservation =>
                    reservation.IdRoom == room.IdRoom &&
                    reservation.Date_from < dateTo &&
                    reservation.Date_to > dateFrom))
            .Select(room => room.IdRoom)
            .ToListAsync();
    }
}
