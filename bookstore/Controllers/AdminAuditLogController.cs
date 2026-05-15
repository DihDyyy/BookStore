using bookstore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/AuditLog")]
    public class AdminAuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminAuditLogController(ApplicationDbContext context) { _context = context; }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? action, string? entityType, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var query = _context.AuditLogs.AsQueryable();
            if (!string.IsNullOrEmpty(action)) query = query.Where(l => l.Action.Contains(action));
            if (!string.IsNullOrEmpty(entityType)) query = query.Where(l => l.EntityType == entityType);
            if (fromDate.HasValue) query = query.Where(l => l.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(l => l.CreatedAt <= toDate.Value.AddDays(1));

            ViewBag.TotalItems = await query.CountAsync();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(await query.CountAsync() / 20.0);
            ViewBag.Action = action; ViewBag.EntityType = entityType; ViewBag.FromDate = fromDate; ViewBag.ToDate = toDate;

            var logs = await query.OrderByDescending(l => l.CreatedAt).Skip((page - 1) * 20).Take(20).ToListAsync();
            return View("~/Views/Admin/AuditLog/Index.cshtml", logs);
        }
    }
}
