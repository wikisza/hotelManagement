using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class RoomDescriptionOption
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<RoomDescription> RoomDescriptions { get; set; } = new List<RoomDescription>();
    }
}
