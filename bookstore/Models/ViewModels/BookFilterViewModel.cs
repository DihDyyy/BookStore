namespace bookstore.Models.ViewModels
{
    public class BookFilterViewModel
    {
        public List<Book> Books { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Author> Authors { get; set; } = new();
        public List<Publisher> Publishers { get; set; } = new();

        // Filter values
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int? PublisherId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
    }
}
