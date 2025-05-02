using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class Standard
    {
        [Key]
        public int IdStandard { get; set; }
        [Required]
        public required string StandardName { get; set; }
        public float StandardValue { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
