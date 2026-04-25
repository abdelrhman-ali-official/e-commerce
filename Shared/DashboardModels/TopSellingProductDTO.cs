namespace Shared.DashboardModels
{
    public class TopSellingProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; } // Revenue - Cost
        public int OrderCount { get; set; }
        public decimal AveragePrice { get; set; }
    }
}
