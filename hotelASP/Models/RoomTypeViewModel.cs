using hotelASP.Entities;

namespace hotelASP.Models
{
    public class RoomTypeViewModel
    {
        public RoomType NewRoomType { get; set; } = new RoomType();
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    }
}
