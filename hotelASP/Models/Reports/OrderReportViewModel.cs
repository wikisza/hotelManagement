namespace hotelASP.Models.Reports
{
    public class OrderReportViewModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalOrdersValue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<OrderStatusData> OrdersByStatus { get; set; } = new();
        public List<PopularMenuItemData> PopularItems { get; set; } = new();
        public List<DailyOrderData> DailyBreakdown { get; set; } = new();
    }

    public class OrderStatusData
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class PopularMenuItemData
    {
        public string ItemName { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyOrderData
    {
        public DateTime Date { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalValue { get; set; }
    }
}