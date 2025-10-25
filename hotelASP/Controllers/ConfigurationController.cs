using hotelASP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Controllers
{
    public class ConfigurationController(ApplicationDbContext context) : Controller
    {
        [HttpGet("/migrate")]
        public async Task<IActionResult> MigrateDb()
        {
            await context.Database.MigrateAsync();
            return Ok("migrated");
        }

        [HttpGet("/roomAccess")]
        public IActionResult Index()
        {
            var rooms = context.Rooms.ToList();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult CheckAccess(int roomId, string keyCode)
        {
            var reservation = context.Reservations
                .FirstOrDefault(r => r.IdRoom == roomId && r.KeyCode == keyCode &&
                                     r.Date_from <= DateTime.Now && r.Date_to >= DateTime.Now);

            if (reservation != null)
                return Json(new { success = true, message = "Dostęp przyznany — drzwi otwarte!" });
            else
                return Json(new { success = false, message = "Brak dostępu — karta niepasująca lub brak aktywnej rezerwacji." });
        }
    }
}
