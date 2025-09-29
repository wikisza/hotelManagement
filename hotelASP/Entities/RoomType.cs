using System.ComponentModel.DataAnnotations;

namespace hotelASP.Entities
{
    public class RoomType
    {
        [Key]
        public int IdType { get; set; }
        public int PeopleNumber { get; set; }
        public int BedNumber { get; set; }
        public float BasePrice { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public string DisplayName => $"{PeopleNumber} {(PeopleNumber == 1 ? "osoba" : "osoby")} - {BedNumber} {(BedNumber == 1 ? "łóżko" : "łóżka")}";
    }
}
