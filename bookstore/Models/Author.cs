using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        [StringLength(100)]
        [Display(Name = "Tên tác giả")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Tiểu sử")]
        public string? Bio { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<Book>? Books { get; set; }
    }
}
