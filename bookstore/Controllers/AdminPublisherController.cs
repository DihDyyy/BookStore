using bookstore.Data;
using bookstore.Models;
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

        public AdminPublisherController(ApplicationDbContext context) { _context = context; }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var publishers = await _context.Publishers.Include(p => p.Books).ToListAsync();
            return View("~/Views/Admin/Publisher/Index.cshtml", publishers);
        }

        [HttpGet("Create")]
        public IActionResult Create() => View("~/Views/Admin/Publisher/Create.cshtml");

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            if (!ModelState.IsValid) return View("~/Views/Admin/Publisher/Create.cshtml", publisher);
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm nhà xuất bản thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var pub = await _context.Publishers.FindAsync(id);
            if (pub == null) return NotFound();
            return View("~/Views/Admin/Publisher/Edit.cshtml", pub);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publisher publisher)
        {
            if (id != publisher.Id) return NotFound();
            if (!ModelState.IsValid) return View("~/Views/Admin/Publisher/Edit.cshtml", publisher);
            _context.Update(publisher);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật nhà xuất bản thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var pub = await _context.Publishers.Include(p => p.Books).FirstOrDefaultAsync(p => p.Id == id);
            if (pub == null) return NotFound();
            if (pub.Books != null && pub.Books.Any())
            {
                TempData["Error"] = "Không thể xóa NXB có sách!";
                return RedirectToAction("Index");
            }
            _context.Publishers.Remove(pub);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa nhà xuất bản thành công!";
            return RedirectToAction("Index");
        }
    }
}
