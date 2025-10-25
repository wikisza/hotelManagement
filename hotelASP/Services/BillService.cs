using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services
{
    public class BillsService : IBillService
    {
        private readonly ApplicationDbContext _context;

        public BillsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bill> CreateBillForReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id_reservation == reservationId);

            if (reservation == null)
            {
                throw new KeyNotFoundException("Nie można utworzyć rachunku, ponieważ rezerwacja nie została znaleziona.");
            }

            if (reservation.Room == null)
            {
                throw new InvalidOperationException("Nie można utworzyć rachunku, ponieważ rezerwacja nie ma przypisanego pokoju.");
            }

            var numberOfNights = Math.Ceiling((reservation.Date_to - reservation.Date_from).TotalDays);

            var totalAmount = numberOfNights * reservation.Room.Price;

            var newBill = new Bill
            {
                ReservationId = reservationId,
                BillDate = DateTime.UtcNow,
                Amount = (decimal)totalAmount,
                Status = "Otwarty"
            };

            await _context.Bills.AddAsync(newBill);

            return newBill;
        }
    }
}