using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Mvc;

namespace bookstore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        public CartController(ApplicationDbContext context, ICartService cartService, ICouponService couponService) { _context = context; _cartService = cartService; _couponService = couponService; }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            ViewBag.Total = _cartService.GetCartTotal();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int bookId, int quantity = 1, bool ajax = false)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) { if (ajax) return Json(new { success = false, message = "Sách không tồn tại" }); TempData["Error"] = "Sách không tồn tại"; return RedirectToAction("Index", "Home"); }
            if (quantity > book.Stock) { if (ajax) return Json(new { success = false, message = "Số lượng vượt quá tồn kho" }); TempData["Error"] = "Số lượng vượt quá tồn kho"; return RedirectToAction("Details", "Book", new { id = bookId }); }
            _cartService.AddToCart(new CartItem { BookId = book.Id, Title = book.Title, Price = book.EffectivePrice, Image = book.Image, Quantity = quantity });
            if (ajax) return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cartCount = _cartService.GetCartCount(), cartTotal = _cartService.GetCartTotal() });
            TempData["Success"] = "Đã thêm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int bookId, int quantity, bool ajax = false)
        {
            _cartService.UpdateQuantity(bookId, quantity);
            if (ajax) { var cart = _cartService.GetCart(); var item = cart.FirstOrDefault(c => c.BookId == bookId); return Json(new { success = true, cartCount = _cartService.GetCartCount(), cartTotal = _cartService.GetCartTotal(), itemTotal = item?.Total ?? 0 }); }
            TempData["Success"] = "Đã cập nhật giỏ hàng!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int bookId, bool ajax = false)
        {
            _cartService.RemoveFromCart(bookId);
            if (ajax) return Json(new { success = true, cartCount = _cartService.GetCartCount(), cartTotal = _cartService.GetCartTotal() });
            TempData["Success"] = "Đã xóa sách khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var total = _cartService.GetCartTotal();
            var (isValid, message, discount) = await _couponService.ValidateCoupon(couponCode, total);
            return Json(new { success = isValid, message, discount, finalTotal = total - discount, couponCode = couponCode?.Trim().ToUpper() });
        }

        [HttpGet]
        public IActionResult GetCount() => Json(new { count = _cartService.GetCartCount() });
    }
}
