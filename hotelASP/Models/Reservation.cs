using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models
{
	public class Reservation
	{
		[Key]
		public int Id_reservation { get; set; }
		[Required]
		public DateTime Date_from { get; set; }
		[Required]
		public DateTime Date_to { get; set; }
		[Required]
		public required string First_name { get; set; }
		[Required]
		public required string Last_name { get; set; }
		[Required]
		public int IdRoom { get; set; }
		public Room? Room { get; set; }
	}
}
