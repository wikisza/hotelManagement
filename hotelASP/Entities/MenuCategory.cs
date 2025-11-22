using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class MenuCategory
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
