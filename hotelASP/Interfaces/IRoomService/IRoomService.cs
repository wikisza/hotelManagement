using hotelASP.Entities;
using hotelASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Interfaces.IRoomService
{
    public interface IRoomService
    {
        Task UpdateRoomStatuses();
        
        //Rooms
        Task<Room> CreateRoom(CreateRoomViewModel model);
        Task<Room> UpdateRoom(int id, Room room, int[] SelectedOptions);
        Task<bool> DeleteRoom(int? id);
        Task<List<Room>> GetAllRooms();
        Task<Room?> GetRoomById(int id);


        //Standards
        Task<Standard> CreateStandard(StandardViewModel vm);
        Task<List<Standard>> GetAllStandards();


        //Types
        Task<List<RoomType>> GetAllRoomTypes();
        Task<RoomType> CreateRoomType(RoomTypeViewModel vm);
        Task<RoomType> UpdateRoomType(RoomType roomType);
        Task<bool> DeleteRoomType(int IdType);


        //Descriptions
        Task<List<RoomDescription>> GetAllRoomDescription();
        Task<List<RoomDescriptionOption>> GetAllRoomDescriptionOption();
        Task<RoomDescriptionOption> CreateRoomDescriptionOption(RoomDescriptionViewModel vm);


        //Check Room Status
        Task<bool> CheckRoomStatus(string KeyCode);
    }
}
