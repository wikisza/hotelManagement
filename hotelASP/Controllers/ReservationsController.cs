using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReservationService _reservationService;

        public ReservationsController(ApplicationDbContext context, IReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
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
            var result = await _reservationService.FindReservation(id ?? 0);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return RedirectToAction(nameof(Index));
            }

            return View(await _context.Reservations.FindAsync(id));
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_reservation,Date_from, Date_to, First_name, Last_name, IdRoom, KeyCode")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var result = await _reservationService.CreateAsync(reservation);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(reservation);
                }

                return RedirectToAction(nameof(Index));

            }

            return View(reservation);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var result = await _reservationService.FindReservation(id ?? 0);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return RedirectToAction(nameof(Index));
            }

            return View(await _context.Reservations.FindAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_reservation,Date_from, Date_to, First_name, Last_name, IdRoom, KeyCode")] Reservation reservation)
        {
            if (id != reservation.Id_reservation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _reservationService.EditAsync(reservation);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(reservation);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _reservationService.FindReservation(id ?? 0);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return RedirectToAction(nameof(Index));
            }

            return View(await _context.Reservations.FindAsync(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reservationService.DeleteConfirmed(id);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return RedirectToAction(nameof(Index));
            }
            
            return RedirectToAction(nameof(Index));
        }

        
    }
}

