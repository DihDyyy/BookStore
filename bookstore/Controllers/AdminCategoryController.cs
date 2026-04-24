using bookstore.Data;
using bookstore.Models;
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

        public AdminCategoryController(ApplicationDbContext context) { _context = context; }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.Include(c => c.Books).ToListAsync();
            return View("~/Views/Admin/Category/Index.cshtml", categories);
        }

        [HttpGet("Create")]
        public IActionResult Create() => View("~/Views/Admin/Category/Create.cshtml");

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Category/Create.cshtml", category);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm danh mục thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View("~/Views/Admin/Category/Edit.cshtml", cat);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();
            if (!ModelState.IsValid) return View("~/Views/Admin/Category/Edit.cshtml", category);

            _context.Update(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _context.Categories.Include(c => c.Books).FirstOrDefaultAsync(c => c.Id == id);
            if (cat == null) return NotFound();
            if (cat.Books != null && cat.Books.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục có sách!";
                return RedirectToAction("Index");
            }
            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
