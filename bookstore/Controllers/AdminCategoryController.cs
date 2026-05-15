using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Category")]
    public class AdminCategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLog;
        public AdminCategoryController(ApplicationDbContext context, IAuditLogService auditLog) { _context = context; _auditLog = auditLog; }

        [HttpGet("")] public async Task<IActionResult> Index() { var cats = await _context.Categories.Include(c => c.Books).OrderBy(c => c.DisplayOrder).ToListAsync(); return View("~/Views/Admin/Category/Index.cshtml", cats); }
        [HttpGet("Create")] public IActionResult Create() => View("~/Views/Admin/Category/Create.cshtml");

        [HttpPost("Create")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category) { if (!ModelState.IsValid) return View("~/Views/Admin/Category/Create.cshtml", category); _context.Categories.Add(category); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Thêm danh mục", "Category", category.Id, category.Name); TempData["Success"] = "Thêm danh mục thành công!"; return RedirectToAction("Index"); }

        [HttpGet("Edit/{id}")] public async Task<IActionResult> Edit(int id) { var cat = await _context.Categories.FindAsync(id); if (cat == null) return NotFound(); return View("~/Views/Admin/Category/Edit.cshtml", cat); }

        [HttpPost("Edit/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category) { if (id != category.Id) return NotFound(); if (!ModelState.IsValid) return View("~/Views/Admin/Category/Edit.cshtml", category); _context.Update(category); await _context.SaveChangesAsync(); await _auditLog.LogAsync("Sửa danh mục", "Category", id, category.Name); TempData["Success"] = "Cập nhật thành công!"; return RedirectToAction("Index"); }

        [HttpPost("Delete/{id}")] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) { var cat = await _context.Categories.FindAsync(id); if (cat == null) return NotFound(); cat.IsDeleted = true; await _context.SaveChangesAsync(); await _auditLog.LogAsync("Xóa danh mục", "Category", id, cat.Name); TempData["Success"] = "Xóa thành công!"; return RedirectToAction("Index"); }
    }
}
