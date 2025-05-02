using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public ICollection<UserAccount> Users { get; set; }
    }
}
