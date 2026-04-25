namespace Shared.DashboardModels
{
    public class SalesAnalyticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; } // Revenue - Cost (excluding shipping)
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal AverageOrderProfit { get; set; }
        public List<DailySalesDTO> DailySales { get; set; } = new List<DailySalesDTO>();
        public List<MonthlySalesDTO> MonthlySales { get; set; } = new List<MonthlySalesDTO>();
    }

    public class DailySalesDTO
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; } // Revenue - Cost (excluding shipping)
    }

    public class MonthlySalesDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; } // Revenue - Cost (excluding shipping)
    }
}
