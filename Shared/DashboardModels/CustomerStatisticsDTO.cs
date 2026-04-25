namespace Shared.DashboardModels
{
    public class CustomerStatisticsDTO
    {
        public int TotalCustomers { get; set; }
        public int TotalGuestOrders { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int ActiveCustomers { get; set; } // Customers with at least 1 order
        public List<TopCustomerDTO> TopCustomers { get; set; } = new List<TopCustomerDTO>();
        public List<CustomerGrowthDTO> CustomerGrowth { get; set; } = new List<CustomerGrowthDTO>();
    }

    public class TopCustomerDTO
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    public class CustomerGrowthDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int NewCustomers { get; set; }
    }
}
