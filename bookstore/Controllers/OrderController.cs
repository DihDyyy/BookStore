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

        public OrderController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: /Order/Checkout
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "";
            var viewModel = new CheckoutViewModel
            {
                FullName = userName,
                CartItems = cart,
                TotalPrice = _cartService.GetCartTotal()
            };

            return View(viewModel);
        }

        // POST: /Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = _cartService.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            model.CartItems = cart;
            model.TotalPrice = _cartService.GetCartTotal();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Create order
            var order = new Order
            {
                UserId = userId,
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                Note = model.Note,
                TotalPrice = model.TotalPrice,
                Status = "Chờ xử lý",
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order details
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);

                // Update stock
                var book = await _context.Books.FindAsync(item.BookId);
                if (book != null)
                {
                    book.Stock -= item.Quantity;
                    if (book.Stock < 0) book.Stock = 0;
                }
            }

            await _context.SaveChangesAsync();

            // Clear cart
            _cartService.ClearCart();

            return RedirectToAction("Success", new { id = order.Id });
        }

        // GET: /Order/Success/5
        public async Task<IActionResult> Success(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        // GET: /Order/History
        public async Task<IActionResult> History()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await _context.Orders
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }
    }
}
