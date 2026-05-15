using System.ComponentModel.DataAnnotations;

namespace bookstore.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ tên người nhận")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(300)]
        [Display(Name = "Địa chỉ giao hàng")]
        public string Address { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD";

        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0;

        public int? SelectedAddressId { get; set; }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice => TotalPrice - DiscountAmount;

        public List<UserAddress> SavedAddresses { get; set; } = new();
    }
}
