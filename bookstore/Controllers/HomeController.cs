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
        public HomeController(ApplicationDbContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var banners = await _context.Banners.Where(b => b.IsActive).OrderBy(b => b.DisplayOrder).ToListAsync();
            var newBooks = await _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Reviews).OrderByDescending(b => b.CreatedAt).Take(8).ToListAsync();
            var featuredBooks = await _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Reviews).Where(b => b.IsFeatured).Take(8).ToListAsync();
            var flashSaleBooks = await _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Reviews).Where(b => b.SalePrice.HasValue && b.SaleEndDate.HasValue && b.SaleEndDate > DateTime.Now).OrderBy(b => b.SaleEndDate).Take(8).ToListAsync();

            var bestSellerIds = await _context.OrderDetails.GroupBy(od => od.BookId).Select(g => new { BookId = g.Key, Total = g.Sum(od => od.Quantity) }).OrderByDescending(x => x.Total).Take(8).Select(x => x.BookId).ToListAsync();
            List<Book> bestSellers;
            if (bestSellerIds.Any())
                bestSellers = await _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Reviews).Where(b => bestSellerIds.Contains(b.Id)).ToListAsync();
            else
                bestSellers = await _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Reviews).OrderBy(b => b.Id).Take(8).ToListAsync();

            var categories = await _context.Categories.Include(c => c.Books).OrderBy(c => c.DisplayOrder).ToListAsync();

            return View(new HomeViewModel { Banners = banners, NewBooks = newBooks, BestSellers = bestSellers, FeaturedBooks = featuredBooks, FlashSaleBooks = flashSaleBooks, Categories = categories });
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
