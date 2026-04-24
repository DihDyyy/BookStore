namespace bookstore.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int TodayOrders { get; set; }

        public List<Order> RecentOrders { get; set; } = new();
        public List<TopBookViewModel> TopBooks { get; set; } = new();
        public List<MonthlyRevenueViewModel> MonthlyRevenue { get; set; } = new();
    }

    public class TopBookViewModel
    {
        public string Title { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MonthlyRevenueViewModel
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}
