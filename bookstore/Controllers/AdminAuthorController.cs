using bookstore.Data;
using bookstore.Models;
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

        public AdminAuthorController(ApplicationDbContext context) { _context = context; }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors.Include(a => a.Books).ToListAsync();
            return View("~/Views/Admin/Author/Index.cshtml", authors);
        }

        [HttpGet("Create")]
        public IActionResult Create() => View("~/Views/Admin/Author/Create.cshtml");

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Author/Create.cshtml", author);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm tác giả thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();
            return View("~/Views/Admin/Author/Edit.cshtml", author);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id) return NotFound();
            if (!ModelState.IsValid) return View("~/Views/Admin/Author/Edit.cshtml", author);
            _context.Update(author);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật tác giả thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return NotFound();
            if (author.Books != null && author.Books.Any())
            {
                TempData["Error"] = "Không thể xóa tác giả có sách!";
                return RedirectToAction("Index");
            }
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa tác giả thành công!";
            return RedirectToAction("Index");
        }
    }
}
