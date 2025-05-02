using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Controllers
{
    public class ReservationsController : Controller, IReservationsController
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpGet("/get_current_reservations")]
        public JsonResult GetReservations()
        {
            var now = DateTime.Now;
            var reservations = _context.Reservations
                .Where(r => r.Date_to > now)
                .Include(r => r.Room)
                .Select(r => new
                {
                    start = r.Date_from.Date.AddHours(14).ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = r.Date_to.Date.AddHours(10).ToString("yyyy-MM-ddTHH:mm:ss"),
                    title = r.First_name + ' ' + r.Last_name + ", pokój: " + r.Room.RoomNumber,
                    description = $"Pokój: {r.Room.RoomNumber}",
                    IdRoom = r.IdRoom
                })
                .ToList();

            return Json(reservations);
        }

        [HttpGet("/get_old_reservations")]
        public JsonResult GetOldReservations()
        {
            var now = DateTime.Now;
            var oldReservations = _context.Reservations
                .Where(r => r.Date_to <= now)
                .Select(r => new
                {
                    start = r.Date_from.Date.AddHours(14).ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = r.Date_to.Date.AddHours(10).ToString("yyyy-MM-ddTHH:mm:ss"),
                    title = r.First_name + ' ' + r.Last_name + ", pokój: " + r.IdRoom,
                    description = $"Pokój: {r.IdRoom}",
                    IdRoom = r.IdRoom
                })
                .ToList();

            return Json(oldReservations);
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableRooms(DateTime dateFrom, DateTime dateTo)
        {
            var availableRooms = await _context.Rooms
                .Where(room => !_context.Reservations
                    .Any(reservation =>
                        reservation.IdRoom == room.IdRoom &&
                        reservation.Date_from < dateTo &&
                        reservation.Date_to > dateFrom))
                .Select(room => new
                {
                    room.IdRoom,
                    room.RoomNumber,
                    room.Description
                })
                .ToListAsync();

            return Json(availableRooms);
        }


        public IActionResult Calendar()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }




        public async Task<IActionResult> CurrentReservations()
        {
            var today = DateTime.Now;

            var reservations = await _context.Reservations
                .Where(r => r.Date_to >= today)
                .OrderBy(r => r.Date_from)
                .ToListAsync();

            return View(reservations);
        }

        public async Task<IActionResult> HistoryReservations()
        {
            var yesterday = DateTime.Now;

            var reservations = await _context.Reservations
                .Where(r => r.Date_to <= yesterday)
                .OrderByDescending(r => r.Date_from)
                .ToListAsync();
            return View(reservations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Select(r => new
                {
                    r.Id_reservation,
                    r.Date_from,
                    r.Date_to,
                    CheckIn = r.Date_from.Date.AddHours(14),
                    CheckOut = r.Date_to.Date.AddHours(10),
                    r.First_name,
                    r.Last_name,
                    r.IdRoom
                })
                .FirstOrDefaultAsync(m => m.Id_reservation == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_reservation,Date_from, Date_to, First_name, Last_name, IdRoom, KeyCode")] Reservation reservation)
        {
            if (reservation.Date_from.TimeOfDay != new TimeSpan(14, 0, 0))
            {
                reservation.Date_from = reservation.Date_from.Date.AddHours(14); 
            }

            if (reservation.Date_to.TimeOfDay != new TimeSpan(10, 0, 0))
            {
                reservation.Date_to = reservation.Date_to.Date.AddHours(10);
            }

            if (ModelState.IsValid)
            {
                var overlappingReservations = await _context.Reservations
                    .Where(r => r.IdRoom == reservation.IdRoom &&
                                r.Date_from < reservation.Date_to &&
                                r.Date_to > reservation.Date_from)
                    .ToListAsync();

                if (overlappingReservations.Any())
                {
                    ModelState.AddModelError(string.Empty, "Pokój jest już zajęty w wybranym terminie.");
                    return View(reservation);
                }

                _context.Add(reservation);

                var thisRoom = await _context.Rooms.FindAsync(reservation.IdRoom);

                thisRoom.IsEmpty=true;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_reservation,Date_from, Date_to, First_name, Last_name, IdRoom")] Reservation reservation)
        {
            if (id != reservation.Id_reservation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id_reservation))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.Id_reservation == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id_reservation == id);
        }
    }
}

