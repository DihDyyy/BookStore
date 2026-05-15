using bookstore.Data;
using bookstore.Models;
using bookstore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Book")]
    public class AdminBookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAuditLogService _auditLog;

        public AdminBookController(ApplicationDbContext context, IFileUploadService fileUploadService, IAuditLogService auditLog)
        { _context = context; _fileUploadService = fileUploadService; _auditLog = auditLog; }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher).OrderByDescending(b => b.CreatedAt).ToListAsync();
            return View("~/Views/Admin/Book/Index.cshtml", books);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create() { await PopulateDropdowns(); return View("~/Views/Admin/Book/Create.cshtml"); }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile? imageFile)
        {
            ModelState.Remove("Image"); ModelState.Remove("Author"); ModelState.Remove("Publisher"); ModelState.Remove("Category");
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View("~/Views/Admin/Book/Create.cshtml", book); }
            if (imageFile != null) book.Image = await _fileUploadService.UploadFileAsync(imageFile, "books");
            book.CreatedAt = DateTime.Now;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Thêm sách", "Book", book.Id, $"Thêm sách: {book.Title}");
            TempData["Success"] = "Thêm sách thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            await PopulateDropdowns();
            return View("~/Views/Admin/Book/Edit.cshtml", book);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, IFormFile? imageFile)
        {
            if (id != book.Id) return NotFound();
            ModelState.Remove("Image"); ModelState.Remove("Author"); ModelState.Remove("Publisher"); ModelState.Remove("Category");
            if (!ModelState.IsValid) { await PopulateDropdowns(); return View("~/Views/Admin/Book/Edit.cshtml", book); }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null) return NotFound();

            existingBook.Title = book.Title; existingBook.Price = book.Price; existingBook.Description = book.Description;
            existingBook.AuthorId = book.AuthorId; existingBook.PublisherId = book.PublisherId; existingBook.CategoryId = book.CategoryId;
            existingBook.PublicationYear = book.PublicationYear; existingBook.Stock = book.Stock;
            existingBook.IsFeatured = book.IsFeatured; existingBook.SalePrice = book.SalePrice; existingBook.SaleEndDate = book.SaleEndDate;

            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(existingBook.Image)) _fileUploadService.DeleteFile(existingBook.Image);
                existingBook.Image = await _fileUploadService.UploadFileAsync(imageFile, "books");
            }
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Sửa sách", "Book", id, $"Sửa sách: {book.Title}");
            TempData["Success"] = "Cập nhật sách thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            book.IsDeleted = true; // Soft delete
            await _context.SaveChangesAsync();
            await _auditLog.LogAsync("Xóa sách", "Book", id, $"Xóa sách: {book.Title}");
            TempData["Success"] = "Xóa sách thành công!";
            return RedirectToAction("Index");
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Authors = new SelectList(await _context.Authors.ToListAsync(), "Id", "Name");
            ViewBag.Publishers = new SelectList(await _context.Publishers.ToListAsync(), "Id", "Name");
        }
    }
}
