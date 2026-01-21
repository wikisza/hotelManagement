namespace hotelASP.Models.Reports
{
    public class CustomerReportViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public List<CustomerDetailData> TopCustomers { get; set; } = new();
        public List<MonthlyCustomerData> MonthlyBreakdown { get; set; } = new();
    }

    public class CustomerDetailData
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int ReservationsCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastVisit { get; set; }
    }

    public class MonthlyCustomerData
    {
        public string Month { get; set; }
        public int NewCustomers { get; set; }
        public int TotalVisits { get; set; }
    }
}