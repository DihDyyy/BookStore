using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }

        // Navigation
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}
