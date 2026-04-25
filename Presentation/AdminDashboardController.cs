using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation
{
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("admin")]
    public class AdminDashboardController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminDashboardController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get overall dashboard overview with key metrics
        /// </summary>
        [HttpGet("overview")]
        [ResponseCache(Duration = 120, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetDashboardOverview()
        {
            var overview = await _serviceManager.DashboardService.GetDashboardOverviewAsync();
            return Ok(overview);
        }

        /// <summary>
        /// Get sales analytics with daily and monthly breakdown
        /// </summary>
        /// <param name="startDate">Start date (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date (optional, defaults to today)</param>
        [HttpGet("sales-analytics")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "startDate", "endDate" })]
        public async Task<IActionResult> GetSalesAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var analytics = await _serviceManager.DashboardService.GetSalesAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }

        /// <summary>
        /// Get top selling products
        /// </summary>
        /// <param name="topCount">Number of top products to return (default: 10)</param>
        [HttpGet("top-selling-products")]
        [ResponseCache(Duration = 180, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "topCount" })]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int topCount = 10)
        {
            var topProducts = await _serviceManager.DashboardService.GetTopSellingProductsAsync(topCount);
            return Ok(topProducts);
        }

        /// <summary>
        /// Get customer statistics including top customers and growth
        /// </summary>
        [HttpGet("customer-statistics")]
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetCustomerStatistics()
        {
            var statistics = await _serviceManager.DashboardService.GetCustomerStatisticsAsync();
            return Ok(statistics);
        }

        /// <summary>
        /// Get inventory alerts for low stock and out of stock products
        /// </summary>
        /// <param name="lowStockThreshold">Threshold for low stock alert (default: 10)</param>
        [HttpGet("inventory-alerts")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "lowStockThreshold" })]
        public async Task<IActionResult> GetInventoryAlerts([FromQuery] int lowStockThreshold = 10)
        {
            var alerts = await _serviceManager.DashboardService.GetInventoryAlertsAsync(lowStockThreshold);
            return Ok(alerts);
        }
    }
}
