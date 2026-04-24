using bookstore.Data;
using bookstore.Models;
using bookstore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 12;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Book
        public async Task<IActionResult> Index(int? categoryId, int? authorId, int? publisherId,
            decimal? minPrice, decimal? maxPrice, string? searchQuery, string? sortBy, int page = 1)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .AsQueryable();

            // Filter by category
            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId.Value);

            // Filter by author
            if (authorId.HasValue)
                query = query.Where(b => b.AuthorId == authorId.Value);

            // Filter by publisher
            if (publisherId.HasValue)
                query = query.Where(b => b.PublisherId == publisherId.Value);

            // Filter by price range
            if (minPrice.HasValue)
                query = query.Where(b => b.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(b => b.Price <= maxPrice.Value);

            // Search by title or author name
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    (b.Author != null && b.Author.Name.Contains(searchQuery)));
            }

            // Sort
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(b => b.Price),
                "price_desc" => query.OrderByDescending(b => b.Price),
                "name_asc" => query.OrderBy(b => b.Title),
                "name_desc" => query.OrderByDescending(b => b.Title),
                "newest" => query.OrderByDescending(b => b.CreatedAt),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var books = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new BookFilterViewModel
            {
                Books = books,
                Categories = await _context.Categories.ToListAsync(),
                Authors = await _context.Authors.ToListAsync(),
                Publishers = await _context.Publishers.ToListAsync(),
                CategoryId = categoryId,
                AuthorId = authorId,
                PublisherId = publisherId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SearchQuery = searchQuery,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems
            };

            return View(viewModel);
        }

        // GET: /Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            // Related books (same category)
            ViewBag.RelatedBooks = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.CategoryId == book.CategoryId && b.Id != book.Id)
                .Take(4)
                .ToListAsync();

            return View(book);
        }

        // GET: /Book/Search (AJAX)
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
                return Json(new List<object>());

            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Title.Contains(query) ||
                           (b.Author != null && b.Author.Name.Contains(query)))
                .Take(5)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    AuthorName = b.Author != null ? b.Author.Name : "",
                    b.Price,
                    b.Image
                })
                .ToListAsync();

            return Json(books);
        }
    }
}
