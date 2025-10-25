namespace hotelASP.Models
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime BillDate { get; set; }
    }
}