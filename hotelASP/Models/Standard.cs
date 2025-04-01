using hotelASP.Entities;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
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
