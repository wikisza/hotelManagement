namespace hotelASP.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public int TotalReservations { get; set; }
        public int TotalOrders { get; set; }
    }
}