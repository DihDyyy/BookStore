using bookstore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Order")]
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrderController(ApplicationDbContext context) { _context = context; }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? status = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();

            ViewBag.CurrentStatus = status;
            return View("~/Views/Admin/Order/Index.cshtml", orders);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View("~/Views/Admin/Order/Details.cshtml", order);
        }

        [HttpPost("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var validStatuses = new[] { "Chờ xử lý", "Đang giao", "Đã giao", "Đã hủy" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ!";
                return RedirectToAction("Details", new { id });
            }

            // If cancelling, restore stock
            if (status == "Đã hủy" && order.Status != "Đã hủy")
            {
                var orderDetails = await _context.OrderDetails
                    .Where(od => od.OrderId == id)
                    .ToListAsync();

                foreach (var detail in orderDetails)
                {
                    var book = await _context.Books.FindAsync(detail.BookId);
                    if (book != null)
                        book.Stock += detail.Quantity;
                }
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Details", new { id });
        }
    }
}
