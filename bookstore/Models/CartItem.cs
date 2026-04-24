namespace bookstore.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
