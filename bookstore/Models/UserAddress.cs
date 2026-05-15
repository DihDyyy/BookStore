using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class UserAddress
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

        [StringLength(100)]
        [Display(Name = "Thành phố")]
        public string? City { get; set; }

        [StringLength(100)]
        [Display(Name = "Quận/Huyện")]
        public string? District { get; set; }

        [StringLength(100)]
        [Display(Name = "Phường/Xã")]
        public string? Ward { get; set; }

        [Display(Name = "Địa chỉ mặc định")]
        public bool IsDefault { get; set; } = false;

        // Navigation
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
