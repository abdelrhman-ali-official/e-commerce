using AutoMapper;
using Domain.Contracts;
using Domain.Entities.OrderEntities;
using Domain.Entities.ProductEntities;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions;
using Shared.DashboardModels;
using System.Globalization;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public DashboardService(IUnitOFWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<DashboardOverviewDTO> GetDashboardOverviewAsync()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var yearStart = new DateTime(now.Year, 1, 1);
            var lastMonthStart = monthStart.AddMonths(-1);

            // Get all orders and order items
            var allOrders = await _unitOfWork.GetRepository<Order, int>().GetAllAsync();
            var allOrderItems = await _unitOfWork.GetRepository<OrderItem, int>().GetAllAsync();

            // Today stats
            var todayOrders = allOrders.Where(o => o.CreatedAt >= todayStart).ToList();
            var todayOrderIds = todayOrders.Select(o => o.Id).ToList();
            var todayRevenue = todayOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice);
            var todayProfit = allOrderItems
                .Where(oi => todayOrderIds.Contains(oi.OrderId) && todayOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity);

            // This month stats
            var monthOrders = allOrders.Where(o => o.CreatedAt >= monthStart).ToList();
            var monthOrderIds = monthOrders.Select(o => o.Id).ToList();
            var monthRevenue = monthOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice);
            var monthProfit = allOrderItems
                .Where(oi => monthOrderIds.Contains(oi.OrderId) && monthOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity);

            // Last month stats for growth
            var lastMonthOrders = allOrders.Where(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < monthStart).ToList();
            var lastMonthOrderIds = lastMonthOrders.Select(o => o.Id).ToList();
            var lastMonthRevenue = lastMonthOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice);
            var lastMonthProfit = allOrderItems
                .Where(oi => lastMonthOrderIds.Contains(oi.OrderId) && lastMonthOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity);

            // This year stats
            var yearOrders = allOrders.Where(o => o.CreatedAt >= yearStart).ToList();
            var yearOrderIds = yearOrders.Select(o => o.Id).ToList();
            var yearRevenue = yearOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice);
            var yearProfit = allOrderItems
                .Where(oi => yearOrderIds.Contains(oi.OrderId) && yearOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity);

            // Calculate growth percentages
            var revenueGrowth = lastMonthRevenue > 0 
                ? ((monthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 
                : 0;
            var profitGrowth = lastMonthProfit > 0 
                ? ((monthProfit - lastMonthProfit) / lastMonthProfit) * 100 
                : 0;
            var orderGrowth = lastMonthOrders.Count > 0 
                ? ((decimal)(monthOrders.Count - lastMonthOrders.Count) / lastMonthOrders.Count) * 100 
                : 0;

            // Product stats
            var allProducts = await _unitOfWork.GetRepository<Product, int>().GetAllAsync();

            // Customer count
            var customerCount = _userManager.Users.Count();

            // Pending orders
            var pendingOrdersCount = allOrders.Count(o => o.Status == OrderStatus.Pending);

            return new DashboardOverviewDTO
            {
                TodayRevenue = todayRevenue,
                TodayProfit = todayProfit,
                MonthRevenue = monthRevenue,
                MonthProfit = monthProfit,
                YearRevenue = yearRevenue,
                YearProfit = yearProfit,
                TodayOrders = todayOrders.Count,
                MonthOrders = monthOrders.Count,
                YearOrders = yearOrders.Count,
                TotalProducts = allProducts.Count(),
                LowStockProducts = 0, // Not tracked in current schema
                OutOfStockProducts = 0, // Not tracked in current schema
                TotalCustomers = customerCount,
                PendingOrders = pendingOrdersCount,
                RevenueGrowthPercentage = revenueGrowth,
                ProfitGrowthPercentage = profitGrowth,
                OrderGrowthPercentage = orderGrowth
            };
        }

        public async Task<SalesAnalyticsDTO> GetSalesAnalyticsAsync(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-6);
            var end = endDate ?? DateTime.UtcNow;

            var orders = await _unitOfWork.GetRepository<Order, int>().GetAllAsync();
            var allOrderItems = await _unitOfWork.GetRepository<OrderItem, int>().GetAllAsync();
            var filteredOrders = orders.Where(o => o.CreatedAt >= start && o.CreatedAt <= end).ToList();
            var filteredOrderIds = filteredOrders.Select(o => o.Id).ToList();
            var filteredOrderItems = allOrderItems.Where(oi => filteredOrderIds.Contains(oi.OrderId)).ToList();

            // Calculate overall stats
            var totalRevenue = filteredOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice);
            var totalProfit = filteredOrderItems
                .Where(oi => filteredOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity);
            var totalOrders = filteredOrders.Count;
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
            var averageOrderProfit = totalOrders > 0 ? totalProfit / totalOrders : 0;

            // Get unique customers
            var customerIds = filteredOrders.Where(o => !string.IsNullOrEmpty(o.UserId)).Select(o => o.UserId).Distinct().Count();

            // Status breakdown
            var pendingCount = filteredOrders.Count(o => o.Status == OrderStatus.Pending);
            var confirmedCount = filteredOrders.Count(o => o.Status == OrderStatus.Confirmed);
            var shippingCount = filteredOrders.Count(o => o.Status == OrderStatus.Shipping);
            var deliveredCount = filteredOrders.Count(o => o.Status == OrderStatus.Delivered);
            var cancelledCount = filteredOrders.Count(o => o.Status == OrderStatus.Cancelled);

            // Daily sales
            var dailySales = filteredOrders
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice),
                    OrderIds = g.Select(o => o.Id).ToList()
                })
                .Select(g => new DailySalesDTO
                {
                    Date = g.Date,
                    OrderCount = g.OrderCount,
                    Revenue = g.Revenue,
                    Profit = filteredOrderItems
                        .Where(oi => g.OrderIds.Contains(oi.OrderId) && filteredOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                        .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity)
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Monthly sales
            var monthlySales = filteredOrders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    OrderCount = g.Count(),
                    Revenue = g.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice),
                    OrderIds = g.Select(o => o.Id).ToList()
                })
                .Select(g => new MonthlySalesDTO
                {
                    Year = g.Year,
                    Month = g.Month,
                    MonthName = g.MonthName,
                    OrderCount = g.OrderCount,
                    Revenue = g.Revenue,
                    Profit = filteredOrderItems
                        .Where(oi => g.OrderIds.Contains(oi.OrderId) && filteredOrders.First(o => o.Id == oi.OrderId).Status != OrderStatus.Cancelled)
                        .Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity)
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToList();

            return new SalesAnalyticsDTO
            {
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                TotalOrders = totalOrders,
                TotalCustomers = customerIds,
                PendingOrders = pendingCount,
                ConfirmedOrders = confirmedCount,
                ShippingOrders = shippingCount,
                DeliveredOrders = deliveredCount,
                CancelledOrders = cancelledCount,
                AverageOrderValue = averageOrderValue,
                AverageOrderProfit = averageOrderProfit,
                DailySales = dailySales,
                MonthlySales = monthlySales
            };
        }

        public async Task<IEnumerable<TopSellingProductDTO>> GetTopSellingProductsAsync(int topCount = 10)
        {
            var orderItems = await _unitOfWork.GetRepository<OrderItem, int>().GetAllAsync();

            var topProducts = orderItems
                .GroupBy(oi => new { oi.ProductId, oi.ProductName, oi.ProductPictureUrl })
                .Select(g => new TopSellingProductDTO
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    PictureUrl = g.Key.ProductPictureUrl ?? string.Empty,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity),
                    TotalProfit = g.Sum(oi => (oi.Price - oi.CostPrice) * oi.Quantity),
                    OrderCount = g.Count(),
                    AveragePrice = g.Average(oi => oi.Price)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(topCount)
                .ToList();

            return topProducts;
        }

        public async Task<CustomerStatisticsDTO> GetCustomerStatisticsAsync()
        {
            var totalCustomers = _userManager.Users.Count();
            var orders = await _unitOfWork.GetRepository<Order, int>().GetAllAsync();

            var guestOrders = orders.Count(o => string.IsNullOrEmpty(o.UserId));

            // New customers this month (calculated by first order date)
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var firstOrderDates = orders
                .Where(o => !string.IsNullOrEmpty(o.UserId))
                .GroupBy(o => o.UserId)
                .Select(g => g.Min(o => o.CreatedAt))
                .ToList();
            var newCustomersThisMonth = firstOrderDates.Count(date => date >= monthStart);

            // Active customers (have at least 1 order)
            var activeCustomerIds = orders.Where(o => !string.IsNullOrEmpty(o.UserId)).Select(o => o.UserId).Distinct().ToList();
            var activeCustomers = activeCustomerIds.Count;

            // Top customers
            var topCustomers = orders
                .Where(o => !string.IsNullOrEmpty(o.UserId))
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalPrice),
                    LastOrderDate = g.Max(o => o.CreatedAt)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(10)
                .ToList();

            var topCustomerDTOs = new List<TopCustomerDTO>();
            foreach (var customer in topCustomers)
            {
                var user = await _userManager.FindByIdAsync(customer.UserId!);
                if (user != null)
                {
                    topCustomerDTOs.Add(new TopCustomerDTO
                    {
                        CustomerId = customer.UserId!,
                        CustomerName = user.DisplayName ?? "Unknown",
                        CustomerEmail = user.Email ?? "Unknown",
                        TotalOrders = customer.TotalOrders,
                        TotalSpent = customer.TotalSpent,
                        LastOrderDate = customer.LastOrderDate
                    });
                }
            }

            // Customer growth by month (last 12 months) - based on first order date
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-12);
            var customerGrowth = orders
                .Where(o => !string.IsNullOrEmpty(o.UserId))
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    FirstOrderDate = g.Min(o => o.CreatedAt)
                })
                .Where(x => x.FirstOrderDate >= twelveMonthsAgo)
                .ToList()
                .GroupBy(x => new { x.FirstOrderDate.Year, x.FirstOrderDate.Month })
                .Select(g => new CustomerGrowthDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    NewCustomers = g.Count()
                })
                .OrderBy(c => c.Year).ThenBy(c => c.Month)
                .ToList();

            return new CustomerStatisticsDTO
            {
                TotalCustomers = totalCustomers,
                TotalGuestOrders = guestOrders,
                NewCustomersThisMonth = newCustomersThisMonth,
                ActiveCustomers = activeCustomers,
                TopCustomers = topCustomerDTOs,
                CustomerGrowth = customerGrowth
            };
        }

        public async Task<InventoryAlertDTO> GetInventoryAlertsAsync(int lowStockThreshold = 10)
        {
            // Note: Current Product entity doesn't track StockQuantity
            // Returning empty alerts - can be enhanced when inventory tracking is added
            
            return new InventoryAlertDTO
            {
                LowStockProducts = new List<LowStockProductDTO>(),
                OutOfStockProducts = new List<OutOfStockProductDTO>(),
                TotalLowStockCount = 0,
                TotalOutOfStockCount = 0
            };
        }
    }
}
