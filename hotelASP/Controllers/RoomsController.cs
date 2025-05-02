using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hotelASP.Data;
using hotelASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using hotelASP.Entities;

namespace hotelASP.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            await UpdateRoomStatuses();
            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        public async Task<IActionResult> UpdateRoomStatuses()
        {
            var now = DateTime.Now;

            var rooms = await _context.Rooms.ToListAsync();

            foreach (var room in rooms)
            {
                var hasActiveReservation = await _context.Reservations
                    .AnyAsync(reservation =>
                        reservation.IdRoom == room.IdRoom &&
                        reservation.Date_from <= now &&
                        reservation.Date_to >= now);

                if (hasActiveReservation)
                {
                    room.IsTaken = 1;
                }
                else
                {
                    room.IsTaken = 0;
                }

            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.IdRoom == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        public IActionResult CreateStandard()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStandard([Bind("IdStandard,StandardName,StandardValue")] Standard standard)
        {
            if (ModelState.IsValid)
            {
                _context.Standards.Add(standard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(CreateStandard));
        }

        public IActionResult CreateType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateType([Bind("IdType,PeopleNumber,BedNumber,BasePrice")] RoomType type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(type);
        }

        public IActionResult Create()
        {
            ViewBag.Types = _context.Types.ToList();
            ViewBag.Standards = _context.Standards.ToList();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRoom,RoomNumber, IdStandard, IdType, FloorNumber, Description, Price, IsTaken")] Room room)
        {
            if (ModelState.IsValid)
            {
                var existingRoom = _context.Rooms.Any(r => r.RoomNumber == room.RoomNumber);
                if (existingRoom)
                {
                    ModelState.AddModelError("RoomNumber", "Pokój z tym numerem już istnieje.");
                    return View(room);
                }

                var standardValue = _context.Standards
                    .Where(s => s.IdStandard == room.IdStandard)
                    .Select(s => s.StandardValue)
                    .FirstOrDefault();

                var typeValue = _context.Types
                    .Where(t => t.IdType == room.IdType)
                    .Select(t => t.BasePrice)
                    .FirstOrDefault();

                float finalPrice = standardValue * typeValue;


                if (finalPrice != 0)
                {
                    Room newRoom = new Room
                    {
                        IdRoom = room.IdRoom,
                        RoomNumber = room.RoomNumber,
                        IdStandard = room.IdStandard,
                        IdType = room.IdType,
                        FloorNumber = room.FloorNumber,
                        Description = room.Description,
                        Price = finalPrice,
                        IsTaken = 0
                    };

                    _context.Rooms.Add(newRoom);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View("Błąd przy dodawaniu pokoju", new ErrorViewModel());
                }
            }
            return View(room);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRoom,Description, Is_taken, Price")] Room room)
        {
            if (id != room.IdRoom)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.IdRoom))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Account");
            }
            return View(room);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.IdRoom == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.IdRoom == id);
        }

        public IActionResult CheckRoomStatus()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckRoomStatus(string KeyCode)
        {

            return View();
        }
    }
}
