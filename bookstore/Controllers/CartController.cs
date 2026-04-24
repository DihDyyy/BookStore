using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public CartController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            ViewBag.Total = _cartService.GetCartTotal();
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        public async Task<IActionResult> Add(int bookId, int quantity = 1)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                TempData["Error"] = "Sách không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            if (quantity > book.Stock)
            {
                TempData["Error"] = "Số lượng vượt quá tồn kho";
                return RedirectToAction("Details", "Book", new { id = bookId });
            }

            var cartItem = new CartItem
            {
                BookId = book.Id,
                Title = book.Title,
                Price = book.Price,
                Image = book.Image,
                Quantity = quantity
            };

            _cartService.AddToCart(cartItem);
            TempData["Success"] = "Đã thêm vào giỏ hàng!";

            return RedirectToAction("Index");
        }

        // POST: /Cart/Update
        [HttpPost]
        public IActionResult Update(int bookId, int quantity)
        {
            _cartService.UpdateQuantity(bookId, quantity);
            TempData["Success"] = "Đã cập nhật giỏ hàng!";
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public IActionResult Remove(int bookId)
        {
            _cartService.RemoveFromCart(bookId);
            TempData["Success"] = "Đã xóa sách khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

        // GET: /Cart/GetCount (AJAX)
        [HttpGet]
        public IActionResult GetCount()
        {
            return Json(new { count = _cartService.GetCartCount() });
        }
    }
}
