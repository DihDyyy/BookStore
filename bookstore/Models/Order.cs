using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Giảm giá")]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(50)]
        [Display(Name = "Mã coupon")]
        public string? CouponCode { get; set; }

        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Chờ xử lý";
        // "Chờ xử lý" → "Đang xử lý" → "Đang giao" → "Đã giao"
        // "Đã hủy"

        [StringLength(30)]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD"; // "COD", "VNPay"

        [StringLength(30)]
        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } = "Chưa thanh toán";
        // "Chưa thanh toán", "Đã thanh toán", "Hoàn tiền"

        [StringLength(100)]
        [Display(Name = "Mã giao dịch")]
        public string? TransactionId { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do hủy")]
        public string? CancelReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
