namespace hotelASP.Models.Reports
{
    public class IncomeReportViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal ReservationIncome { get; set; }
        public decimal OrdersIncome { get; set; }
        public int TotalReservations { get; set; }
        public int TotalOrders { get; set; }
        public List<MonthlyIncomeData> MonthlyBreakdown { get; set; } = new();
        public List<RoomTypeIncomeData> RoomTypeBreakdown { get; set; } = new();
    }

    public class MonthlyIncomeData
    {
        public string Month { get; set; }
        public decimal ReservationIncome { get; set; }
        public decimal OrdersIncome { get; set; }
        public decimal TotalIncome { get; set; }
    }

    public class RoomTypeIncomeData
    {
        public string RoomType { get; set; }
        public int ReservationsCount { get; set; }
        public decimal Income { get; set; }
    }
}