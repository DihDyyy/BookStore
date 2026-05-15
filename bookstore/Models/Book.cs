using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [StringLength(200)]
        [Display(Name = "Tên sách")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0, 100000000, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá (VNĐ)")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

        [Display(Name = "Giá gốc (VNĐ)")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal? OriginalPrice { get; set; }

        [Display(Name = "Giá sale (VNĐ)")]
        [Column(TypeName = "decimal(18,0)")]
        public decimal? SalePrice { get; set; }

        [Display(Name = "Ngày kết thúc sale")]
        public DateTime? SaleEndDate { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(255)]
        [Display(Name = "Hình ảnh")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tác giả")]
        [Display(Name = "Tác giả")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà xuất bản")]
        [Display(Name = "Nhà xuất bản")]
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Năm xuất bản")]
        [Range(1900, 2100)]
        public int? PublicationYear { get; set; }

        [Required]
        [Range(0, 100000)]
        [Display(Name = "Số lượng tồn kho")]
        public int Stock { get; set; } = 0;

        [Display(Name = "Sách nổi bật")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Đã xóa")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Ngày thêm")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Computed properties
        [NotMapped]
        public decimal EffectivePrice =>
            SalePrice.HasValue && SaleEndDate.HasValue && SaleEndDate.Value > DateTime.Now
                ? SalePrice.Value
                : Price;

        [NotMapped]
        public bool IsOnSale =>
            SalePrice.HasValue && SaleEndDate.HasValue && SaleEndDate.Value > DateTime.Now;

        [NotMapped]
        public int DiscountPercent =>
            IsOnSale && Price > 0
                ? (int)Math.Round((1 - SalePrice!.Value / Price) * 100)
                : 0;

        // Navigation
        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }

        [ForeignKey("PublisherId")]
        public Publisher? Publisher { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
