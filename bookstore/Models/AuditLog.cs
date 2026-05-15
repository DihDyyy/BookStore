using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [StringLength(100)]
        [Display(Name = "Người thực hiện")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Hành động")]
        public string Action { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Đối tượng")]
        public string EntityType { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        [StringLength(1000)]
        [Display(Name = "Chi tiết")]
        public string? Details { get; set; }

        [StringLength(50)]
        [Display(Name = "Địa chỉ IP")]
        public string? IpAddress { get; set; }

        [Display(Name = "Thời gian")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
