using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces;
using hotelASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReservationService _reservationService;
        private readonly ICustomerService _customerService;

        public ReservationsController(ApplicationDbContext context, IReservationService reservationService, ICustomerService customerService)
        {
            _context = context;
            _reservationService = reservationService;
            _customerService = customerService;
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
            var availableRooms = await _reservationService.GetAvailableRoomIds(dateFrom, dateTo);
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

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // NOWY PROCES TWORZENIA REZERWACJI

        // Krok 1: Wybór dat
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateReservationViewModel
            {
                DateFrom = DateTime.Today.AddHours(14),
                DateTo = DateTime.Today.AddDays(1).AddHours(10)
            };

            return View("CreateStep1", model);
        }

        // Krok 2: Wyświetl dostępne pokoje
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectDates(CreateReservationViewModel model)
        {
            if (model.DateFrom >= model.DateTo)
            {
                ModelState.AddModelError("", "Data zakończenia musi być późniejsza niż data rozpoczęcia.");
                return View("CreateStep1", model);
            }

            // Walidacja - data rozpoczęcia musi być od dzisiaj (porównujemy tylko daty, bez godzin)
            if (model.DateFrom.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "Data rozpoczęcia nie może być wcześniejsza niż dzisiejsza.");
                return View("CreateStep1", model);
            }

            // Pobierz dostępne pokoje jako listę ID
            var availableRoomIds = await _reservationService.GetAvailableRoomIds(model.DateFrom, model.DateTo);

            var availableRooms = await _context.Rooms
                .Include(r => r.Standard)
                .Include(r => r.RoomType)
                .Include(r => r.RoomDescriptions)
                    .ThenInclude(rd => rd.DescriptionOption)
                .Where(r => availableRoomIds.Contains(r.IdRoom))
                .Select(r => new RoomAvailabilityViewModel
                {
                    IdRoom = r.IdRoom,
                    RoomNumber = r.RoomNumber,
                    StandardName = r.Standard.StandardName,
                    PeopleNumber = r.RoomType.PeopleNumber,
                    BedNumber = r.RoomType.BedNumber,
                    Price = r.Price,
                    NumberOfNights = (int)Math.Ceiling((model.DateTo - model.DateFrom).TotalDays),
                    TotalPrice = r.Price * (float)Math.Ceiling((model.DateTo - model.DateFrom).TotalDays),
                    RoomOptions = r.RoomDescriptions.Select(rd => rd.DescriptionOption.Name).ToList()
                })
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            if (!availableRooms.Any())
            {
                ModelState.AddModelError("", "Brak dostępnych pokoi w wybranym terminie.");
                return View("CreateStep1", model);
            }

            model.AvailableRooms = availableRooms;
            return View("CreateStep2", model);
        }

        // Krok 3: Wybór pokoju i opcja wyboru klienta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectRoom(CreateReservationViewModel model)
        {
            if (model.SelectedRoomId == null)
            {
                ModelState.AddModelError("", "Proszę wybrać pokój.");
                
                // Ponownie pobierz dostępne pokoje
                var availableRoomIds = await _reservationService.GetAvailableRoomIds(model.DateFrom, model.DateTo);

                model.AvailableRooms = await _context.Rooms
                    .Include(r => r.Standard)
                    .Include(r => r.RoomType)
                    .Include(r => r.RoomDescriptions)
                        .ThenInclude(rd => rd.DescriptionOption)
                    .Where(r => availableRoomIds.Contains(r.IdRoom))
                    .Select(r => new RoomAvailabilityViewModel
                    {
                        IdRoom = r.IdRoom,
                        RoomNumber = r.RoomNumber,
                        StandardName = r.Standard.StandardName,
                        PeopleNumber = r.RoomType.PeopleNumber,
                        BedNumber = r.RoomType.BedNumber,
                        Price = r.Price,
                        NumberOfNights = (int)Math.Ceiling((model.DateTo - model.DateFrom).TotalDays),
                        TotalPrice = r.Price * (float)Math.Ceiling((model.DateTo - model.DateFrom).TotalDays),
                        RoomOptions = r.RoomDescriptions.Select(rd => rd.DescriptionOption.Name).ToList()
                    })
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync();

                return View("CreateStep2", model);
            }

            // Pobierz listę istniejących klientów
            model.ExistingCustomers = await _customerService.GetAllCustomersAsync();
            model.KeyCode = GenerateKeyCode();

            return View("CreateStep3", model);
        }

        // Krok 4: Zapisz rezerwację
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizeReservation(CreateReservationViewModel model)
        {
            int customerId;

            // Jeśli wybrano istniejącego klienta
            if (!model.IsNewCustomer && model.ExistingCustomerId.HasValue)
            {
                customerId = model.ExistingCustomerId.Value;
                
                // Zaktualizuj datę ostatniej wizyty
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer != null)
                {
                    customer.LastVisitDate = DateTime.Now;
                    await _customerService.UpdateCustomerAsync(customer);
                }
            }
            // Jeśli tworzony jest nowy klient
            else if (model.IsNewCustomer)
            {
                if (!ModelState.IsValid)
                {
                    model.ExistingCustomers = await _customerService.GetAllCustomersAsync();
                    return View("CreateStep3", model);
                }

                var newCustomer = new Customer
                {
                    FirstName = model.FirstName!,
                    LastName = model.LastName!,
                    Email = model.Email!,
                    PhoneNumber = model.PhoneNumber!,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    Notes = model.Notes,
                    CreatedDate = DateTime.Now,
                    LastVisitDate = DateTime.Now
                };

                var createResult = await _customerService.CreateCustomerAsync(newCustomer);
                
                if (!createResult.Success)
                {
                    ModelState.AddModelError("", createResult.ErrorMessage);
                    model.ExistingCustomers = await _customerService.GetAllCustomersAsync();
                    return View("CreateStep3", model);
                }

                customerId = newCustomer.Id;
            }
            else
            {
                ModelState.AddModelError("", "Proszę wybrać istniejącego klienta lub dodać nowego.");
                model.ExistingCustomers = await _customerService.GetAllCustomersAsync();
                return View("CreateStep3", model);
            }

            // Utwórz rezerwację
            var reservation = new Reservation
            {
                Date_from = model.DateFrom,
                Date_to = model.DateTo,
                CustomerId = customerId,
                IdRoom = model.SelectedRoomId!.Value,
                KeyCode = model.KeyCode ?? GenerateKeyCode()
            };

            var result = await _reservationService.CreateAsync(reservation);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                model.ExistingCustomers = await _customerService.GetAllCustomersAsync();
                return View("CreateStep3", model);
            }

            TempData["SuccessMessage"] = "Rezerwacja została pomyślnie utworzona!";
            return RedirectToAction(nameof(Details), new { id = reservation.Id_reservation });
        }

        // Pomocnicza metoda do generowania kodu dostępu
        private string GenerateKeyCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
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
        public async Task<IActionResult> Edit(int id, ReservationViewModel model)
        {
            if (id != model.Id_reservation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var reservation = new Reservation
                {
                    Id_reservation = model.Id_reservation,
                    Date_from = model.Date_from,
                    Date_to = model.Date_to,
                    CustomerId = model.CustomerId, // WAŻNE!
                    IdRoom = model.IdRoom,
                    KeyCode = model.KeyCode
                };

                var result = await _reservationService.EditAsync(reservation);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
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

