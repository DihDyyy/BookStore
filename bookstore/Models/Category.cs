using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100)]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Icon")]
        public string? Icon { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        public int DisplayOrder { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<Book>? Books { get; set; }
    }
}
