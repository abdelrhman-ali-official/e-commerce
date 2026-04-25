using Shared.DashboardModels;

namespace Services.Abstractions
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDTO> GetDashboardOverviewAsync();
        Task<SalesAnalyticsDTO> GetSalesAnalyticsAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync(int topCount = 10);
        Task<CustomerStatisticsDTO> GetCustomerStatisticsAsync();
        Task<InventoryAlertDTO> GetInventoryAlertsAsync(int lowStockThreshold = 10);
    }
}
