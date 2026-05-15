using bookstore.Data;
using bookstore.Helpers;
using bookstore.Models;
using bookstore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bookstore.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 12;
        public BookController(ApplicationDbContext context) { _context = context; }

        public async Task<IActionResult> Index(int? categoryId, int? authorId, int? publisherId, decimal? minPrice, decimal? maxPrice, string? searchQuery, string? sortBy, int? minRating, int page = 1)
        {
            var query = _context.Books.Include(b => b.Author).Include(b => b.Publisher).Include(b => b.Category).Include(b => b.Reviews).AsQueryable();
            if (categoryId.HasValue) query = query.Where(b => b.CategoryId == categoryId.Value);
            if (authorId.HasValue) query = query.Where(b => b.AuthorId == authorId.Value);
            if (publisherId.HasValue) query = query.Where(b => b.PublisherId == publisherId.Value);
            if (minPrice.HasValue) query = query.Where(b => b.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(b => b.Price <= maxPrice.Value);
            if (minRating.HasValue) query = query.Where(b => b.Reviews != null && b.Reviews.Any() && b.Reviews.Average(r => r.Rating) >= minRating.Value);

            if (!string.IsNullOrEmpty(searchQuery))
                query = query.Where(b => b.Title.Contains(searchQuery) || (b.Author != null && b.Author.Name.Contains(searchQuery)));

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(b => b.Price),
                "price_desc" => query.OrderByDescending(b => b.Price),
                "name_asc" => query.OrderBy(b => b.Title),
                "name_desc" => query.OrderByDescending(b => b.Title),
                "newest" => query.OrderByDescending(b => b.CreatedAt),
                "rating" => query.OrderByDescending(b => b.Reviews != null && b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            var books = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            var viewModel = new BookFilterViewModel
            {
                Books = books, Categories = await _context.Categories.ToListAsync(), Authors = await _context.Authors.ToListAsync(), Publishers = await _context.Publishers.ToListAsync(),
                CategoryId = categoryId, AuthorId = authorId, PublisherId = publisherId, MinPrice = minPrice, MaxPrice = maxPrice, SearchQuery = searchQuery, SortBy = sortBy,
                CurrentPage = page, TotalPages = totalPages, TotalItems = totalItems
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var book = await _context.Books.Include(b => b.Author).Include(b => b.Publisher).Include(b => b.Category).Include(b => b.Reviews!).ThenInclude(r => r.User).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null) return NotFound();

            ViewBag.RelatedBooks = await _context.Books.Include(b => b.Author).Include(b => b.Reviews).Where(b => b.CategoryId == book.CategoryId && b.Id != book.Id).Take(4).ToListAsync();
            ViewBag.AverageRating = book.Reviews != null && book.Reviews.Any() ? book.Reviews.Average(r => r.Rating) : 0;
            ViewBag.ReviewCount = book.Reviews?.Count ?? 0;

            // Recommendation: people who bought this also bought
            var buyerIds = await _context.OrderDetails.Where(od => od.BookId == id).Select(od => od.Order!.UserId).Distinct().ToListAsync();
            if (buyerIds.Any())
            {
                ViewBag.RecommendedBooks = await _context.OrderDetails.Where(od => buyerIds.Contains(od.Order!.UserId) && od.BookId != id).GroupBy(od => od.BookId).OrderByDescending(g => g.Count()).Take(4).Select(g => g.Key).ToListAsync();
                var recIds = (List<int>)ViewBag.RecommendedBooks;
                ViewBag.RecommendedBooks = await _context.Books.Include(b => b.Author).Include(b => b.Reviews).Where(b => recIds.Contains(b.Id)).ToListAsync();
            }
            else
                ViewBag.RecommendedBooks = new List<Book>();

            return View(book);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int bookId, int rating, string? comment)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.Reviews.FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            if (existing != null)
            {
                existing.Rating = rating;
                existing.Comment = comment;
                existing.CreatedAt = DateTime.Now;
            }
            else
            {
                _context.Reviews.Add(new Review { BookId = bookId, UserId = userId, Rating = rating, Comment = comment, CreatedAt = DateTime.Now });
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đánh giá của bạn đã được ghi nhận!";
            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2) return Json(new List<object>());
            var allBooks = await _context.Books.Include(b => b.Author).Take(200).ToListAsync();
            var results = allBooks.Where(b => VietnameseHelper.ContainsVietnamese(b.Title, query) || (b.Author != null && VietnameseHelper.ContainsVietnamese(b.Author.Name, query)))
                .Take(5).Select(b => new { b.Id, b.Title, AuthorName = b.Author?.Name ?? "", b.Price, b.Image, EffectivePrice = b.EffectivePrice, IsOnSale = b.IsOnSale }).ToList();
            return Json(results);
        }
    }
}
