using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Banner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả ngắn")]
        public string? Subtitle { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [StringLength(500)]
        [Display(Name = "Liên kết")]
        public string? LinkUrl { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
