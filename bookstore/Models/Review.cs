using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Đánh giá từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rating { get; set; }

        [StringLength(1000)]
        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }

        public bool IsApproved { get; set; } = true;

        [Display(Name = "Ngày đánh giá")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
