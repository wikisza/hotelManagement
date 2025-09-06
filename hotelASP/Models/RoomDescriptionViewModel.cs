using hotelASP.Entities;

namespace hotelASP.Models
{
    public class RoomDescriptionViewModel
    {
        public RoomDescriptionOption NewOption { get; set; } = new RoomDescriptionOption();
        public List<RoomDescriptionOption> ExistingOptions { get; set; } = new List<RoomDescriptionOption>();
    }
}
