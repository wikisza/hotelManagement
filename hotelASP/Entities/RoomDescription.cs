using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class RoomDescription
    {
        public int IdRoom { get; set; }
        public Room? Room { get; set; }
        public int IdOption { get; set; }
        public RoomDescriptionOption? DescriptionOption { get; set; }
}
}
