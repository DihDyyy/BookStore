using bookstore.Data;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/User")]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLog;
        public AdminUserController(ApplicationDbContext context, IAuditLogService auditLog) { _context = context; _auditLog = auditLog; }

        [HttpGet("")] public async Task<IActionResult> Index() => View("~/Views/Admin/User/Index.cshtml", await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync());

        [HttpPost("ToggleLock/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id) {
            var user = await _context.Users.FindAsync(id); if (user == null) return NotFound();
            user.IsLocked = !user.IsLocked; await _context.SaveChangesAsync();
            await _auditLog.LogAsync(user.IsLocked ? "Khóa tài khoản" : "Mở khóa tài khoản", "User", id, user.Email);
            TempData["Success"] = user.IsLocked ? "Đã khóa tài khoản!" : "Đã mở khóa tài khoản!"; return RedirectToAction("Index");
        }

        [HttpPost("Delete/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) { var user = await _context.Users.FindAsync(id); if (user == null) return NotFound(); user.IsDeleted = true; await _context.SaveChangesAsync(); await _auditLog.LogAsync("Xóa tài khoản", "User", id, user.Email); TempData["Success"] = "Xóa thành công!"; return RedirectToAction("Index"); }
    }
}
