using System.ComponentModel.DataAnnotations;

namespace bookstore.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhà xuất bản")]
        [StringLength(100)]
        [Display(Name = "Tên nhà xuất bản")]
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Book>? Books { get; set; }
    }
}
