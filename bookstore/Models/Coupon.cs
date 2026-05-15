using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã coupon")]
        [StringLength(50)]
        [Display(Name = "Mã coupon")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Loại giảm giá")]
        public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "Fixed"

        [Required]
        [Range(0, 100000000)]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Giá trị giảm")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn hàng tối thiểu")]
        public decimal MinOrderAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Giảm tối đa")]
        public decimal? MaxDiscount { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Giới hạn sử dụng")]
        public int UsageLimit { get; set; } = 100;

        [Display(Name = "Đã sử dụng")]
        public int UsedCount { get; set; } = 0;

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
