namespace Shared.DashboardModels
{
    public class DashboardOverviewDTO
    {
        public decimal TodayRevenue { get; set; }
        public decimal TodayProfit { get; set; } // Revenue - Cost (excluding shipping)
        public decimal MonthRevenue { get; set; }
        public decimal MonthProfit { get; set; }
        public decimal YearRevenue { get; set; }
        public decimal YearProfit { get; set; }
        public int TodayOrders { get; set; }
        public int MonthOrders { get; set; }
        public int YearOrders { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int PendingOrders { get; set; }
        public decimal RevenueGrowthPercentage { get; set; }
        public decimal ProfitGrowthPercentage { get; set; }
        public decimal OrderGrowthPercentage { get; set; }
    }
}
