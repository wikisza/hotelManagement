using hotelASP.Data;
using hotelASP.Entities;
using hotelASP.Interfaces.IRoomService;
using hotelASP.Models;
using hotelASP.Views.Rooms.Helpers;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Services.RoomService
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Rooms
        public async Task UpdateRoomStatuses()
        {
            var now = DateTime.Now;
            var rooms = await _context.Rooms.ToListAsync();

            foreach (var room in rooms)
            {
                var hasActiveReservation = await _context.Reservations
                    .AnyAsync(r => r.IdRoom == room.IdRoom &&
                                   r.Date_from <= now &&
                                   r.Date_to >= now);

                room.IsTaken = hasActiveReservation ? 1 : 0;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Room>> GetAllRooms()
        {
            return await _context.Rooms
                .Include(s => s.Standard)
                .Include(rt => rt.RoomType)
                .Include(r => r.RoomDescriptions)
                    .ThenInclude(rd => rd.DescriptionOption)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomById(int id)
        {
            return await _context.Rooms
                .Include(s => s.Standard)
                .Include(rt => rt.RoomType)
                .Include(r => r.RoomDescriptions)
                    .ThenInclude(rd => rd.DescriptionOption)
                .FirstOrDefaultAsync(r => r.IdRoom == id);
        }

        public async Task<Room> CreateRoom(CreateRoomViewModel model)
        {
            var existingRoom = await _context.Rooms
                .Where(x => x.RoomNumber == model.Rooms.RoomNumber)
                .FirstOrDefaultAsync();

            if (existingRoom is not null)
            {
                throw new InvalidOperationException("Pokój o tym numerze już istnieje.");
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

            var newRoom = new Room
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

            return newRoom;
        }

        public async Task<Room> UpdateRoom(int id, Room room, int[] selectedOptions)
        {
            var existingRoom = await _context.Rooms
                .Include(r => r.RoomDescriptions)
                .FirstOrDefaultAsync(r => r.IdRoom == id);

            if (existingRoom == null)
                throw new KeyNotFoundException("Pokój nie istnieje");

            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.IdStandard = room.IdStandard;
            existingRoom.IdType = room.IdType;
            existingRoom.FloorNumber = room.FloorNumber;
            existingRoom.Price = room.Price;
            existingRoom.IsTaken = room.IsTaken;

            _context.RoomDescriptions.RemoveRange(existingRoom.RoomDescriptions);

            if (selectedOptions != null)
            {
                foreach (var optId in selectedOptions)
                {
                    _context.RoomDescriptions.Add(new RoomDescription
                    {
                        IdRoom = id,
                        IdOption = optId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return existingRoom;
        }

        public async Task<bool> DeleteRoom(int? id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Standards
        public async Task<List<Standard>> GetAllStandards()
        {
            return await _context.Standards.ToListAsync();
        }

        public async Task<Standard> CreateStandard(StandardViewModel vm)
        {
            _context.Standards.Add(vm.NewStandard);
            await _context.SaveChangesAsync();
            return vm.NewStandard;
        }

        #endregion

        #region Room Types
        public async Task<List<RoomType>> GetAllRoomTypes()
        {
            return await _context.Types.ToListAsync();
        }

        public async Task<RoomType> CreateRoomType(RoomTypeViewModel vm)
        {
            _context.Types.Add(vm.NewRoomType);
            await _context.SaveChangesAsync();
            return vm.NewRoomType;
        }

        public async Task<RoomType> UpdateRoomType(RoomType roomType)
        {
            _context.Update(roomType);
            await _context.SaveChangesAsync();
            return roomType;
        }

        public async Task<bool> DeleteRoomType(int idType)
        {
            var roomType = await _context.Types.FindAsync(idType);
            if (roomType == null) return false;

            _context.Types.Remove(roomType);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Room Descriptions

        public async Task<List<RoomDescription>> GetAllRoomDescription()
        {
            return await _context.RoomDescriptions.ToListAsync();
        }

        public async Task<List<RoomDescriptionOption>> GetAllRoomDescriptionOption()
        {
            return await _context.RoomDescriptionOptions.ToListAsync();
        }

        public async Task<RoomDescriptionOption> CreateRoomDescriptionOption(RoomDescriptionViewModel vm)
        {
            _context.RoomDescriptionOptions.Add(vm.NewOption);
            await _context.SaveChangesAsync();
            return vm.NewOption;
        }
        #endregion

        // --- CHECK ROOM STATUS ---
        public async Task<bool> CheckRoomStatus(string keyCode)
        {
            // Tu możesz dodać logikę wyszukiwania pokoju po kodzie
            //var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Code == keyCode);
            //return room != null && room.IsTaken == 1;
            return true;
        }

        
    }
}
