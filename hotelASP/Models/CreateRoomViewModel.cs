using hotelASP.Entities;

namespace hotelASP.Models
{
    public class CreateRoomViewModel
    {
        public Room Rooms { get; set; } = new Room { Price = 0.0f };
        public List<Standard> Standards { get; set; } = new List<Standard>();
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public List<RoomDescription> RoomDescriptions { get; set; } = new List<RoomDescription>();
        public List<RoomDescriptionOption> RoomDescriptionOptions { get; set; } = new List<RoomDescriptionOption>();

        public int[] SelectedOptions { get; set; }
    }
}
