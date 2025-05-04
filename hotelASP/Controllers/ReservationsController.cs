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
        public async Task<JsonResult> GetReservations()
        {
            var reservations = await _reservationService.GetReservations();
            return Json(reservations);
        }

        [HttpGet("/get_old_reservations")]
        public async Task<JsonResult> GetOldReservations()
        {
            var reservations = await _reservationService.GetOldReservations();
            return Json(reservations);
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableRooms(DateTime dateFrom, DateTime dateTo)
        {
            var availableRooms = await _reservationService.GetAvailableRooms(dateFrom, dateTo);
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




        public IActionResult CurrentReservations()
        {
            var reservations = _reservationService.CurrentReservations();
            return View(reservations);
        }

        public IActionResult HistoryReservations()
        {
            var reservations = _reservationService.HistoryReservations();
            return View(reservations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var reservation = await _reservationService.FindReservation(id ?? 0);

            if (reservation==null)
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
            var reservation = await _reservationService.FindReservation(id ?? 0);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
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
            var reservation = await _reservationService.FindReservation(id ?? 0);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
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

