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

        public AdminBookController(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        // GET: /Admin/Book
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View("~/Views/Admin/Book/Index.cshtml", books);
        }

        // GET: /Admin/Book/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View("~/Views/Admin/Book/Create.cshtml");
        }

        // POST: /Admin/Book/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile? imageFile)
        {
            ModelState.Remove("Image");
            ModelState.Remove("Author");
            ModelState.Remove("Publisher");
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View("~/Views/Admin/Book/Create.cshtml", book);
            }

            if (imageFile != null)
            {
                book.Image = await _fileUploadService.UploadFileAsync(imageFile, "books");
            }

            book.CreatedAt = DateTime.Now;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm sách thành công!";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Book/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            await PopulateDropdowns();
            return View("~/Views/Admin/Book/Edit.cshtml", book);
        }

        // POST: /Admin/Book/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, IFormFile? imageFile)
        {
            if (id != book.Id) return NotFound();

            ModelState.Remove("Image");
            ModelState.Remove("Author");
            ModelState.Remove("Publisher");
            ModelState.Remove("Category");

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View("~/Views/Admin/Book/Edit.cshtml", book);
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null) return NotFound();

            existingBook.Title = book.Title;
            existingBook.Price = book.Price;
            existingBook.Description = book.Description;
            existingBook.AuthorId = book.AuthorId;
            existingBook.PublisherId = book.PublisherId;
            existingBook.CategoryId = book.CategoryId;
            existingBook.PublicationYear = book.PublicationYear;
            existingBook.Stock = book.Stock;

            if (imageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(existingBook.Image))
                    _fileUploadService.DeleteFile(existingBook.Image);

                existingBook.Image = await _fileUploadService.UploadFileAsync(imageFile, "books");
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật sách thành công!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/Book/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            // Delete image file
            if (!string.IsNullOrEmpty(book.Image))
                _fileUploadService.DeleteFile(book.Image);

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

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
