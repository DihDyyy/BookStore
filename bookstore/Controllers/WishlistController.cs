using bookstore.Data;
using bookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bookstore.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Wishlist
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var wishlists = await _context.Wishlists
                .Include(w => w.Book!)
                    .ThenInclude(b => b.Author)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return View(wishlists);
        }

        // POST: /Wishlist/Toggle
        [HttpPost]
        public async Task<IActionResult> Toggle(int bookId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var existing = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);

            if (existing != null)
            {
                _context.Wishlists.Remove(existing);
                TempData["Success"] = "Đã xóa khỏi danh sách yêu thích!";
            }
            else
            {
                _context.Wishlists.Add(new Wishlist
                {
                    UserId = userId,
                    BookId = bookId,
                    CreatedAt = DateTime.Now
                });
                TempData["Success"] = "Đã thêm vào danh sách yêu thích!";
            }

            await _context.SaveChangesAsync();

            var returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }

        // GET: /Wishlist/Check (AJAX)
        [HttpGet]
        public async Task<IActionResult> Check(int bookId)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Json(new { isInWishlist = false });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.BookId == bookId);

            return Json(new { isInWishlist = exists });
        }
    }
}
