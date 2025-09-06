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
    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ReservationViewModel> FindReservation(int id)
    {
        var reservation = await _context.Reservations
            .Where(r => r.Id_reservation == id)
            .Include(r => r.Room)
            .Select(r => new ReservationViewModel
            {
                Id_reservation = r.Id_reservation,
                Date_from = r.Date_from,
                Date_to = r.Date_to,
                First_name = r.First_name,
                Last_name = r.Last_name,
                IdRoom = r.IdRoom,
                KeyCode = r.KeyCode
            })
            .FirstOrDefaultAsync();

        return reservation;
    }

    public async Task<(bool Success, string ErrorMessage)> CreateAsync(Reservation reservation)
    {
        if (reservation.Date_from.TimeOfDay != new TimeSpan(14, 0, 0))
        {
            reservation.Date_from = reservation.Date_from.Date.AddHours(14);
        }

        if (reservation.Date_to.TimeOfDay != new TimeSpan(10, 0, 0))
        {
            reservation.Date_to = reservation.Date_to.Date.AddHours(10);
        }

        //checking if the room is already reserved in the given date range
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

        var thisRoom = await _context.Rooms.FindAsync(reservation.IdRoom);
        if (thisRoom != null)
        {
            thisRoom.IsEmpty = true;
        }

        await _context.SaveChangesAsync();
        return (true, string.Empty);
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
            existingReservation.First_name = reservation.First_name;
            existingReservation.Last_name = reservation.Last_name;
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
            .Include(r => r.Room)
            .Select(r => new ReservationViewModel
            {
                Id_reservation = r.Id_reservation,
                Date_from = r.Date_from,
                Date_to = r.Date_to,
                First_name = r.First_name,
                Last_name = r.Last_name,
                IdRoom = r.IdRoom,
                KeyCode = r.KeyCode
            })
            .ToList();
        return currentReservations;
    }

    public List<ReservationViewModel> HistoryReservations()
    {
        var historyReservations = _context.Reservations
            .Where(r => r.Date_to <= DateTime.Now)
            .OrderByDescending(r => r.Date_from)
            .Include(r => r.Room)
            .Select(r => new ReservationViewModel
            {
                Id_reservation = r.Id_reservation,
                Date_from = r.Date_from,
                Date_to = r.Date_to,
                First_name = r.First_name,
                Last_name = r.Last_name,
                IdRoom = r.IdRoom,
                KeyCode = r.KeyCode
            })
            .ToList();
        return historyReservations;
    }
    public async Task<List<object>> GetReservations()
    {
        var now = DateTime.Now;
        return await _context.Reservations
            .Where(r => r.Date_to > now)
            .Include(r => r.Room)
            .Select(r => new
            {
                start = r.Date_from,
                end = r.Date_to,
                title = r.First_name + ' ' + r.Last_name + ", pokój: " + r.Room.RoomNumber,
                IdRoom = r.IdRoom
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<List<object>> GetOldReservations()
    {
        var now = DateTime.Now;
        return await _context.Reservations
            .Where(r => r.Date_to <= now)
            .Include(r => r.Room)
            .Select(r => new
            {
                start = r.Date_from,
                end = r.Date_to,
                title = r.First_name + ' ' + r.Last_name + ", pokój: " + r.Room.RoomNumber,
                IdRoom = r.IdRoom
            })
            .Cast<object>()
            .ToListAsync();
    }
    public async Task<List<object>> GetAvailableRooms(DateTime dateFrom, DateTime dateTo)
    {
        return await _context.Rooms
            .Where(room => !_context.Reservations
                .Any(reservation =>
                    reservation.IdRoom == room.IdRoom &&
                    reservation.Date_from < dateTo &&
                    reservation.Date_to > dateFrom))
            .Select(room => new
            {
                room.IdRoom,
                room.RoomNumber,
            })
            .Cast<object>()
            .ToListAsync();

    }
}
