using bookstore.Data;
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

        public AdminUserController(ApplicationDbContext context) { _context = context; }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
            return View("~/Views/Admin/User/Index.cshtml", users);
        }

        [HttpPost("ToggleLock/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (user.Role == "Admin")
            {
                TempData["Error"] = "Không thể khóa tài khoản Admin!";
                return RedirectToAction("Index");
            }

            user.IsLocked = !user.IsLocked;
            await _context.SaveChangesAsync();

            TempData["Success"] = user.IsLocked ? "Đã khóa tài khoản!" : "Đã mở khóa tài khoản!";
            return RedirectToAction("Index");
        }
    }
}
