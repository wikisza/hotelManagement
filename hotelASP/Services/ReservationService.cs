using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(bool Success, string ErrorMessage)> FindReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return (false, "Rezerwacja nie została znaleziona.");
            }
            
            return (true, string.Empty);
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
                if(!_context.Reservations.Any(r => r.Id_reservation == reservation.Id_reservation))
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
    }
}
