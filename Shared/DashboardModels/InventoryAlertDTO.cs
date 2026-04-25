namespace Shared.DashboardModels
{
    public class InventoryAlertDTO
    {
        public List<LowStockProductDTO> LowStockProducts { get; set; } = new List<LowStockProductDTO>();
        public List<OutOfStockProductDTO> OutOfStockProducts { get; set; } = new List<OutOfStockProductDTO>();
        public int TotalLowStockCount { get; set; }
        public int TotalOutOfStockCount { get; set; }
    }

    public class LowStockProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
    }

    public class OutOfStockProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public DateTime? LastSoldDate { get; set; }
    }
}
