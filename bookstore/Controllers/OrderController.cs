using bookstore.Data;
using bookstore.Models;
using bookstore.Models.ViewModels;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace bookstore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
        private readonly IVNPayService _vnPayService;
        private readonly IEmailService _emailService;

        public OrderController(ApplicationDbContext context, ICartService cartService, ICouponService couponService, IVNPayService vnPayService, IEmailService emailService)
        { _context = context; _cartService = cartService; _couponService = couponService; _vnPayService = vnPayService; _emailService = emailService; }

        public async Task<IActionResult> Checkout()
        {
            var cart = _cartService.GetCart();
            if (!cart.Any()) { TempData["Error"] = "Giỏ hàng trống!"; return RedirectToAction("Index", "Cart"); }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var addresses = await _context.UserAddresses.Where(a => a.UserId == userId).OrderByDescending(a => a.IsDefault).ToListAsync();
            var defaultAddr = addresses.FirstOrDefault(a => a.IsDefault) ?? addresses.FirstOrDefault();

            var viewModel = new CheckoutViewModel
            {
                FullName = defaultAddr?.FullName ?? User.FindFirstValue(ClaimTypes.Name) ?? "",
                Phone = defaultAddr?.Phone ?? "",
                Address = defaultAddr?.Address ?? "",
                CartItems = cart, TotalPrice = _cartService.GetCartTotal(),
                SavedAddresses = addresses
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = _cartService.GetCart();
            if (!cart.Any()) { TempData["Error"] = "Giỏ hàng trống!"; return RedirectToAction("Index", "Cart"); }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            model.CartItems = cart;
            model.TotalPrice = _cartService.GetCartTotal();
            model.SavedAddresses = await _context.UserAddresses.Where(a => a.UserId == userId).ToListAsync();

            // Apply coupon if provided
            if (!string.IsNullOrWhiteSpace(model.CouponCode))
            {
                var (isValid, _, discount) = await _couponService.ValidateCoupon(model.CouponCode, model.TotalPrice);
                if (isValid) model.DiscountAmount = discount;
            }

            ModelState.Remove("CartItems"); ModelState.Remove("SavedAddresses");
            if (!ModelState.IsValid) return View(model);

            var order = new Order
            {
                UserId = userId, FullName = model.FullName, Phone = model.Phone, Address = model.Address,
                Note = model.Note, TotalPrice = model.FinalPrice, DiscountAmount = model.DiscountAmount,
                CouponCode = model.CouponCode?.Trim().ToUpper(), PaymentMethod = model.PaymentMethod,
                PaymentStatus = model.PaymentMethod == "COD" ? "Chưa thanh toán" : "Chờ thanh toán",
                Status = "Chờ xử lý", CreatedAt = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                _context.OrderDetails.Add(new OrderDetail { OrderId = order.Id, BookId = item.BookId, Quantity = item.Quantity, Price = item.Price });
                var book = await _context.Books.FindAsync(item.BookId);
                if (book != null) { book.Stock -= item.Quantity; if (book.Stock < 0) book.Stock = 0; }
            }
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(model.CouponCode))
                await _couponService.IncrementUsage(model.CouponCode);

            _cartService.ClearCart();

            // VNPay redirect
            if (model.PaymentMethod == "VNPay")
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                var payUrl = _vnPayService.CreatePaymentUrl(order.Id, model.FinalPrice, $"Thanh toan don hang #{order.Id}", ip);
                return Redirect(payUrl);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
                await _emailService.SendOrderConfirmationAsync(email, order.Id.ToString(), order.TotalPrice);

            return RedirectToAction("Success", new { id = order.Id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> VNPayCallback()
        {
            if (_vnPayService.ValidateCallback(Request.Query))
            {
                var responseCode = _vnPayService.GetResponseCode(Request.Query);
                var txnRef = Request.Query["vnp_TxnRef"].ToString();
                if (int.TryParse(txnRef, out var orderId))
                {
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        order.TransactionId = _vnPayService.GetTransactionId(Request.Query);
                        if (responseCode == "00") { order.PaymentStatus = "Đã thanh toán"; TempData["Success"] = "Thanh toán thành công!"; }
                        else { order.PaymentStatus = "Thanh toán thất bại"; order.Status = "Đã hủy"; TempData["Error"] = "Thanh toán thất bại!"; }
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Success", new { id = orderId });
                    }
                }
            }
            TempData["Error"] = "Có lỗi xảy ra khi xử lý thanh toán.";
            return RedirectToAction("History");
        }

        public async Task<IActionResult> Success(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders.Include(o => o.OrderDetails!).ThenInclude(od => od.Book).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order == null) return NotFound();
            return View(order);
        }

        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _context.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders.Include(o => o.OrderDetails!).ThenInclude(od => od.Book).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? reason)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order == null) return NotFound();
            if (order.Status != "Chờ xử lý") { TempData["Error"] = "Chỉ có thể hủy đơn hàng đang chờ xử lý!"; return RedirectToAction("Details", new { id }); }

            order.Status = "Đã hủy"; order.CancelReason = reason; order.CancelledAt = DateTime.Now;
            if (order.OrderDetails != null)
                foreach (var detail in order.OrderDetails) { var book = await _context.Books.FindAsync(detail.BookId); if (book != null) book.Stock += detail.Quantity; }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã hủy đơn hàng thành công!";
            return RedirectToAction("Details", new { id });
        }
    }
}
