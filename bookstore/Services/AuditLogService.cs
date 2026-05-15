using bookstore.Data;
using bookstore.Models;
using System.Security.Claims;

namespace bookstore.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string entityType, int? entityId = null, string? details = null);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entityType, int? entityId = null, string? details = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = httpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "System";
            var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

            var log = new AuditLog
            {
                UserId = userId != null ? int.Parse(userId) : null,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
