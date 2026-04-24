using bookstore.Data;
using bookstore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var startOfYear = new DateTime(now.Year, 1, 1);

            var completedOrders = _context.Orders.Where(o => o.Status == "Đã giao");

            var viewModel = new DashboardViewModel
            {
                TotalBooks = await _context.Books.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(u => u.Role == "User"),
                TotalRevenue = await completedOrders.SumAsync(o => o.TotalPrice),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Chờ xử lý"),
                TodayOrders = await _context.Orders.CountAsync(o => o.CreatedAt.Date == now.Date),

                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToListAsync(),

                TopBooks = await _context.OrderDetails
                    .Include(od => od.Book)
                    .Where(od => od.Order != null && od.Order.Status == "Đã giao")
                    .GroupBy(od => new { od.BookId, od.Book!.Title })
                    .Select(g => new TopBookViewModel
                    {
                        Title = g.Key.Title,
                        TotalSold = g.Sum(od => od.Quantity),
                        Revenue = g.Sum(od => od.Quantity * od.Price)
                    })
                    .OrderByDescending(t => t.TotalSold)
                    .Take(5)
                    .ToListAsync(),

                MonthlyRevenue = await _context.Orders
                    .Where(o => o.Status == "Đã giao" && o.CreatedAt >= startOfYear)
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new MonthlyRevenueViewModel
                    {
                        Month = $"T{g.Key.Month}",
                        Revenue = g.Sum(o => o.TotalPrice),
                        OrderCount = g.Count()
                    })
                    .OrderBy(m => m.Month)
                    .ToListAsync()
            };

            return View("~/Views/Admin/Dashboard.cshtml", viewModel);
        }

        // GET: /Admin/GetRevenueData (API for Chart.js)
        [HttpGet]
        public async Task<IActionResult> GetRevenueData()
        {
            var now = DateTime.Now;
            var startOfYear = new DateTime(now.Year, 1, 1);

            var data = await _context.Orders
                .Where(o => o.Status == "Đã giao" && o.CreatedAt >= startOfYear)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    Month = $"Tháng {g.Key.Month}",
                    Revenue = g.Sum(o => o.TotalPrice),
                    Orders = g.Count()
                })
                .OrderBy(m => m.Month)
                .ToListAsync();

            return Json(data);
        }
    }
}
