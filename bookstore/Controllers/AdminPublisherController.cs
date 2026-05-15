using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Publisher")]
    public class AdminPublisherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLog;
        public AdminPublisherController(ApplicationDbContext context, IAuditLogService auditLog) { _context = context; _auditLog = auditLog; }

        [HttpGet("")] public async Task<IActionResult> Index() => View("~/Views/Admin/Publisher/Index.cshtml", await _context.Publishers.Include(p => p.Books).ToListAsync());
        [HttpGet("Create")] public IActionResult Create() => View("~/Views/Admin/Publisher/Create.cshtml");

        [HttpPost("Create")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publisher pub) { if (!ModelState.IsValid) return View("~/Views/Admin/Publisher/Create.cshtml", pub); _context.Publishers.Add(pub); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Thêm NXB", "Publisher", pub.Id, pub.Name); TempData["Success"] = "Thêm NXB thành công!"; return RedirectToAction("Index"); }

        [HttpGet("Edit/{id}")] public async Task<IActionResult> Edit(int id) { var p = await _context.Publishers.FindAsync(id); if (p == null) return NotFound(); return View("~/Views/Admin/Publisher/Edit.cshtml", p); }

        [HttpPost("Edit/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publisher pub) { if (id != pub.Id) return NotFound(); if (!ModelState.IsValid) return View("~/Views/Admin/Publisher/Edit.cshtml", pub); _context.Update(pub); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Sửa NXB", "Publisher", id, pub.Name); TempData["Success"] = "Cập nhật thành công!"; return RedirectToAction("Index"); }

        [HttpPost("Delete/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) { var p = await _context.Publishers.FindAsync(id); if (p == null) return NotFound(); p.IsDeleted = true; await _context.SaveChangesAsync(); await _auditLog.LogAsync("Xóa NXB", "Publisher", id, p.Name); TempData["Success"] = "Xóa thành công!"; return RedirectToAction("Index"); }
    }
}
