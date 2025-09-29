using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces.IRoomService;
using hotelASP.Models;
using hotelASP.Services.RoomService;
using hotelASP.Views.Rooms.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotelASP.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            await _roomService.UpdateRoomStatuses();
            var rooms = await _roomService.GetAllRooms();
            return View(rooms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _roomService.GetRoomById(id.Value);
            if (room == null) return NotFound();

            return View(room);
        }

        public async Task<IActionResult> CreateStandard()
        {
            var vm = new StandardViewModel
            {
                Standards = await _roomService.GetAllStandards()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStandard(StandardViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _roomService.CreateStandard(vm);
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        //TYPES
        public async Task<IActionResult> CreateRoomType()
        {
            var vm = new RoomTypeViewModel
            {
                RoomTypes = await _roomService.GetAllRoomTypes()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoomType(RoomTypeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _roomService.CreateRoomType(vm);
                return RedirectToAction(nameof(CreateRoomType));
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoomType([Bind("IdType,TypeName,PeopleNumber,BedNumber,BasePrice")] RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                await _roomService.UpdateRoomType(roomType);
                return RedirectToAction(nameof(Index));
            }
            return View(roomType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoomType(int IdType)
        {
            await _roomService.DeleteRoomType(IdType);
            return RedirectToAction(nameof(Index));
        }

        //ROOM DESCRIPTION OPTIONS

        public async Task<IActionResult> CreateRoomDescriptionOption()
        {
            var vm = new RoomDescriptionViewModel
            {
                ExistingOptions = await _roomService.GetAllRoomDescriptionOption()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoomDescriptionOption(RoomDescriptionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _roomService.CreateRoomDescriptionOption(vm);
                return RedirectToAction(nameof(CreateRoomDescriptionOption));
            }
            return View(vm);
        }

        //ROOMS
        public async Task<IActionResult> CreateRoomAsync()
        {
            var vm = new CreateRoomViewModel();
            vm.Standards = await _roomService.GetAllStandards();
            vm.RoomTypes = await _roomService.GetAllRoomTypes();
            vm.RoomDescriptionOptions = await _roomService.GetAllRoomDescriptionOption();
            ViewBag.Standards = new SelectList(vm.Standards, "IdStandard", "StandardName");
            ViewBag.RoomTypes = new SelectList(
                vm.RoomTypes
                    .Select(t => new
                    {
                        t.IdType,
                        Display = GrammarHelper.PeopleText(t.PeopleNumber) + " • " +
                                  GrammarHelper.BedsText(t.BedNumber) + " • " +
                                  t.BasePrice + " zł"
                    })
                    .ToList(),
                "IdType",
                "Display"
            );
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(CreateRoomViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _roomService.CreateRoom(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateRoom(int? id)
        {
            if (id == null) return NotFound();

            var room = await _roomService.GetRoomById(id.Value);

            if (room == null) return NotFound();

            var standardList = await _roomService.GetAllStandards();
            var typeList = await _roomService.GetAllRoomTypes();
            ViewBag.Standards = new SelectList(standardList, "IdStandard", "StandardName", room.IdStandard);
            ViewBag.RoomTypes = new SelectList(
                typeList
                    .Select(t => new
                    {
                        t.IdType,
                        Display = GrammarHelper.PeopleText(t.PeopleNumber) + " • " +
                                  GrammarHelper.BedsText(t.BedNumber) + " • " +
                                  t.BasePrice + " zł"
                    })
                    .ToList(),
                "IdType",
                "Display",
                room.IdType
            );
            ViewBag.Options = await _roomService.GetAllRoomDescriptionOption();

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoom(int id, Room room, int[] SelectedOptions)
        {
            if (!ModelState.IsValid) return View(room);

            await _roomService.UpdateRoom(id, room, SelectedOptions);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _roomService.GetRoomById(id.Value);
            
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            await _roomService.DeleteRoom(id);
            return RedirectToAction(nameof(Index));
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
