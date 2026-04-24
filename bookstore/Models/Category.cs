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

        // Navigation
        public ICollection<Book>? Books { get; set; }
    }
}
