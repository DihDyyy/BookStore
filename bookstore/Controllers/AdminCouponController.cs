using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Coupon")]
    public class AdminCouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLog;
        public AdminCouponController(ApplicationDbContext context, IAuditLogService auditLog) { _context = context; _auditLog = auditLog; }

        [HttpGet("")] public async Task<IActionResult> Index() => View("~/Views/Admin/Coupon/Index.cshtml", await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync());
        [HttpGet("Create")] public IActionResult Create() => View("~/Views/Admin/Coupon/Create.cshtml");

        [HttpPost("Create")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon) { coupon.Code = coupon.Code.Trim().ToUpper(); if (!ModelState.IsValid) return View("~/Views/Admin/Coupon/Create.cshtml", coupon); _context.Coupons.Add(coupon); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Thêm coupon", "Coupon", coupon.Id, coupon.Code); TempData["Success"] = "Thêm coupon thành công!"; return RedirectToAction("Index"); }

        [HttpGet("Edit/{id}")] public async Task<IActionResult> Edit(int id) { var c = await _context.Coupons.FindAsync(id); if (c == null) return NotFound(); return View("~/Views/Admin/Coupon/Edit.cshtml", c); }

        [HttpPost("Edit/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon) { if (id != coupon.Id) return NotFound(); coupon.Code = coupon.Code.Trim().ToUpper(); if (!ModelState.IsValid) return View("~/Views/Admin/Coupon/Edit.cshtml", coupon); _context.Update(coupon); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Sửa coupon", "Coupon", id, coupon.Code); TempData["Success"] = "Cập nhật thành công!"; return RedirectToAction("Index"); }

        [HttpPost("ToggleActive/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id) { var c = await _context.Coupons.FindAsync(id); if (c == null) return NotFound(); c.IsActive = !c.IsActive; await _context.SaveChangesAsync(); await _auditLog.LogAsync(c.IsActive ? "Kích hoạt coupon" : "Vô hiệu hóa coupon", "Coupon", id, c.Code); TempData["Success"] = c.IsActive ? "Đã kích hoạt!" : "Đã vô hiệu hóa!"; return RedirectToAction("Index"); }

        [HttpPost("Delete/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) { var c = await _context.Coupons.FindAsync(id); if (c == null) return NotFound(); _context.Coupons.Remove(c); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Xóa coupon", "Coupon", id, c.Code); TempData["Success"] = "Xóa thành công!"; return RedirectToAction("Index"); }
    }
}
