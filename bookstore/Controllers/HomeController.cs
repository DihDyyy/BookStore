using bookstore.Data;
using bookstore.Models;
using bookstore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // New books (8 latest)
            var newBooks = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedAt)
                .Take(8)
                .ToListAsync();

            // Best sellers - simplified query for SQL Server 2014 compatibility
            var bestSellerIds = await _context.OrderDetails
                .GroupBy(od => od.BookId)
                .Select(g => new { BookId = g.Key, Total = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.Total)
                .Take(8)
                .Select(x => x.BookId)
                .ToListAsync();

            List<Book> bestSellers;
            if (bestSellerIds.Any())
            {
                bestSellers = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .Where(b => bestSellerIds.Contains(b.Id))
                    .ToListAsync();
            }
            else
            {
                // No orders yet, show random books
                bestSellers = await _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .OrderBy(b => b.Id)
                    .Take(8)
                    .ToListAsync();
            }

            // All categories
            var categories = await _context.Categories
                .Include(c => c.Books)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                NewBooks = newBooks,
                BestSellers = bestSellers,
                Categories = categories
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
