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

        public async Task<IActionResult> CreateStandard()
        {
            var vm = new StandardViewModel
            {
                Standards = await _context.Standards.ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStandard(StandardViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _context.Standards.Add(vm.NewStandard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            vm.Standards = await _context.Standards.ToListAsync();
            return View(vm);
        }


        //TYPES
        public async Task<IActionResult> CreateType()
        {
            var vm = new RoomTypeViewModel
            {
                RoomTypes = await _context.Types.ToListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateType(RoomTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vm.NewRoomType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoomType([Bind("IdType,TypeName,PeopleNumber,BedNumber,BasePrice")] RoomType roomType)
        {
            if (!ModelState.IsValid)
            {
                return View(roomType);
            }

            try
            {
                _context.Update(roomType);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Types.Any(e => e.IdType == roomType.IdType))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(CreateType));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoomType(int IdType)
        {
            var roomType = await _context.Types.FindAsync(IdType);
            if (roomType is not null)
            {
                _context.Remove(roomType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        //ROOM DESCRIPTION OPTIONS

        public async Task<IActionResult> CreateRoomDescriptionOption()
        {
            var vm = new RoomDescriptionViewModel
            {
                ExistingOptions = await _context.RoomDescriptionOptions.ToListAsync()
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoomDescriptionOption(RoomDescriptionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vm.NewOption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateRoomDescriptionOption));
            }
            return View(vm);
        }

        //ROOMS
        public IActionResult Create()
        {
            var createRoomViewModel = new CreateRoomViewModel
            {
                Rooms = new Room { Price = 0.0f },
                Standards = _context.Standards.ToList(),
                RoomTypes = _context.Types.ToList(),
                RoomDescriptions = _context.RoomDescriptions.ToList(),
                RoomDescriptionOptions = _context.RoomDescriptionOptions.ToList()
            };
            return View(createRoomViewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingRoom = _context.Rooms.Any(r => r.RoomNumber == model.Rooms.RoomNumber);
                if (existingRoom)
                {
                    ModelState.AddModelError("Rooms.RoomNumber", "Pokój z tym numerem już istnieje.");
                    model.Standards = _context.Standards.ToList();
                    model.RoomTypes = _context.Types.ToList();
                    model.RoomDescriptionOptions = _context.RoomDescriptionOptions.ToList();
                    return View(model);
                }

                var standardValue = _context.Standards
                    .Where(s => s.IdStandard == model.Rooms.IdStandard)
                    .Select(s => s.StandardValue)
                    .FirstOrDefault();

                var typeValue = _context.Types
                    .Where(t => t.IdType == model.Rooms.IdType)
                    .Select(t => t.BasePrice)
                    .FirstOrDefault();

                float finalPrice = standardValue * typeValue;

                Room newRoom = new Room
                {
                    RoomNumber = model.Rooms.RoomNumber,
                    IdStandard = model.Rooms.IdStandard,
                    IdType = model.Rooms.IdType,
                    FloorNumber = model.Rooms.FloorNumber,
                    Price = finalPrice,
                    IsTaken = 0
                };

                _context.Rooms.Add(newRoom);
                await _context.SaveChangesAsync(); 

                if (model.SelectedOptions != null)
                {
                    foreach (var optionId in model.SelectedOptions)
                    {
                        _context.RoomDescriptions.Add(new RoomDescription
                        {
                            IdRoom = newRoom.IdRoom, 
                            IdOption = optionId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            model.Standards = _context.Standards.ToList();
            model.RoomTypes = _context.Types.ToList();
            model.RoomDescriptionOptions = _context.RoomDescriptionOptions.ToList();

            return View(model);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdRoom,RoomNumber,IdStandard,IdType,FloorNumber, IsTaken, Price, IsEmpty")] Room room)
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
