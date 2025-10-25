using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class MenuCategories
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        public ICollection<MenuItems> MenuItems { get; set; }
    }
}
