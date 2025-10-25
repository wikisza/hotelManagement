using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class MenuItems
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int MenuCategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public bool ContainsAlcohol { get; set; }
        public MenuCategories? MenuCategory { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
}
