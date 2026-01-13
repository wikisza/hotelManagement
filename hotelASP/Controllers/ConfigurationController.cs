using hotelASP.Data;
using hotelASP.Interfaces.IRoomService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRoomService _roomService;

        public ConfigurationController(ApplicationDbContext context, IRoomService roomService)
        {
            _context = context;
            _roomService = roomService;
        }

        [HttpGet("/migrate")]
        public async Task<IActionResult> MigrateDb()
        {
            await _context.Database.MigrateAsync();
            return Ok("migrated");
        }

        [HttpGet("/roomAccess")]
        public async Task<IActionResult> Index()
        {
            // Najpierw zaktualizuj statusy pokojów
            await _roomService.UpdateRoomStatuses();
            
            // Pobierz pokoje z aktualnymi statusami
            var rooms = await _context.Rooms
                .Include(r => r.Standard)
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
            
            return View(rooms);
        }

        [HttpGet]
        public IActionResult CheckAccess(int roomId, string keyCode)
        {
            var now = DateTime.Now;
            
            // Znajdź aktywną rezerwację dla tego pokoju
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.IdRoom == roomId && 
                                     r.KeyCode == keyCode &&
                                     r.Date_from <= now && 
                                     r.Date_to >= now);

            if (reservation != null)
            {
                return Json(new { 
                    success = true, 
                    message = "Dostęp przyznany — drzwi otwarte!" 
                });
            }
            else
            {
                return Json(new { 
                    success = false, 
                    message = "Brak dostępu — karta niepasująca lub brak aktywnej rezerwacji." 
                });
            }
        }
    }
}
