namespace bookstore.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Book> NewBooks { get; set; } = new();
        public List<Book> BestSellers { get; set; } = new();
        public List<Book> FeaturedBooks { get; set; } = new();
        public List<Book> FlashSaleBooks { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Banner> Banners { get; set; } = new();
    }
}
