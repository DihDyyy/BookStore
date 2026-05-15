using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Author")]
    public class AdminAuthorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLog;
        public AdminAuthorController(ApplicationDbContext context, IAuditLogService auditLog) { _context = context; _auditLog = auditLog; }

        [HttpGet("")] public async Task<IActionResult> Index() => View("~/Views/Admin/Author/Index.cshtml", await _context.Authors.Include(a => a.Books).ToListAsync());
        [HttpGet("Create")] public IActionResult Create() => View("~/Views/Admin/Author/Create.cshtml");

        [HttpPost("Create")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author) { if (!ModelState.IsValid) return View("~/Views/Admin/Author/Create.cshtml", author); _context.Authors.Add(author); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Thêm tác giả", "Author", author.Id, author.Name); TempData["Success"] = "Thêm tác giả thành công!"; return RedirectToAction("Index"); }

        [HttpGet("Edit/{id}")] public async Task<IActionResult> Edit(int id) { var a = await _context.Authors.FindAsync(id); if (a == null) return NotFound(); return View("~/Views/Admin/Author/Edit.cshtml", a); }

        [HttpPost("Edit/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author) { if (id != author.Id) return NotFound(); if (!ModelState.IsValid) return View("~/Views/Admin/Author/Edit.cshtml", author); _context.Update(author); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Sửa tác giả", "Author", id, author.Name); TempData["Success"] = "Cập nhật thành công!"; return RedirectToAction("Index"); }

        [HttpPost("Delete/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) { var a = await _context.Authors.FindAsync(id); if (a == null) return NotFound(); a.IsDeleted = true; await _context.SaveChangesAsync(); await _auditLog.LogAsync("Xóa tác giả", "Author", id, a.Name); TempData["Success"] = "Xóa thành công!"; return RedirectToAction("Index"); }
    }
}
