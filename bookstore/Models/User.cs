using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [StringLength(255)]
        [Display(Name = "Ảnh đại diện")]
        public string? Avatar { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = "User"; // "Admin" or "User"

        public bool IsLocked { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        [StringLength(255)]
        public string? PasswordResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserAddress>? Addresses { get; set; }
    }
}
