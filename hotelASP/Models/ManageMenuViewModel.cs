using hotelASP.Entities;
using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
    public class ManageMenuViewModel
    {
        public MenuCategory NewCategory { get; set; } = new MenuCategory
        {
            Name = string.Empty,
        };

        public MenuItem NewMenuItem { get; set; } = new MenuItem
        {
            Name = string.Empty,
            Description = string.Empty,
            Price = 0m,
            IsAvailable = true,
            ContainsAlcohol = false
        };

        public List<MenuCategory> Categories { get; set; } = new List<MenuCategory>();
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}