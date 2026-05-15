using Microsoft.EntityFrameworkCore;
using bookstore.Models;

namespace bookstore.Data
{
    public static class DbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Admin: admin@bookstore.com / Admin@123
            // User: user@bookstore.com / Admin@123
            var hash = "$2a$11$K5GBBRNx0RjPJE9KAstmQeKgHfXsUW7rFm7jJhcjZzFdqzKQ5Fwi6";

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Admin", Email = "admin@bookstore.com", Password = hash, Role = "Admin", CreatedAt = new DateTime(2024, 1, 1) },
                new User { Id = 2, Name = "Nguyễn Văn A", Email = "user@bookstore.com", Password = hash, Role = "User", Phone = "0901234567", CreatedAt = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Văn học Việt Nam", Icon = "fa-feather-pointed", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Văn học nước ngoài", Icon = "fa-globe", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Kinh tế", Icon = "fa-chart-line", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Kỹ năng sống", Icon = "fa-lightbulb", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Khoa học công nghệ", Icon = "fa-microchip", DisplayOrder = 5 },
                new Category { Id = 6, Name = "Thiếu nhi", Icon = "fa-child-reaching", DisplayOrder = 6 },
                new Category { Id = 7, Name = "Giáo trình", Icon = "fa-graduation-cap", DisplayOrder = 7 },
                new Category { Id = 8, Name = "Tâm lý - Triết học", Icon = "fa-brain", DisplayOrder = 8 }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "Nguyễn Nhật Ánh", Bio = "Nhà văn nổi tiếng với các tác phẩm viết cho tuổi trẻ" },
                new Author { Id = 2, Name = "Tô Hoài", Bio = "Nhà văn lớn của nền văn học Việt Nam hiện đại" },
                new Author { Id = 3, Name = "Dale Carnegie", Bio = "Tác giả sách kỹ năng sống nổi tiếng thế giới" },
                new Author { Id = 4, Name = "Robert Kiyosaki", Bio = "Doanh nhân, tác giả sách tài chính cá nhân" },
                new Author { Id = 5, Name = "Paulo Coelho", Bio = "Nhà văn Brazil nổi tiếng toàn cầu" },
                new Author { Id = 6, Name = "Yuval Noah Harari", Bio = "Sử gia và giáo sư tại Hebrew University" },
                new Author { Id = 7, Name = "Nguyễn Ngọc Tư", Bio = "Nhà văn miền Tây Nam Bộ" },
                new Author { Id = 8, Name = "J.K. Rowling", Bio = "Tác giả series Harry Potter" },
                new Author { Id = 9, Name = "Napoleon Hill", Bio = "Tác giả sách tự lực nổi tiếng" },
                new Author { Id = 10, Name = "Trần Đắc Trung", Bio = "Giảng viên CNTT" }
            );

            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "NXB Trẻ" },
                new Publisher { Id = 2, Name = "NXB Kim Đồng" },
                new Publisher { Id = 3, Name = "NXB Tổng hợp TP.HCM" },
                new Publisher { Id = 4, Name = "NXB Lao Động" },
                new Publisher { Id = 5, Name = "NXB Thế Giới" },
                new Publisher { Id = 6, Name = "NXB Giáo Dục" },
                new Publisher { Id = 7, Name = "NXB Hà Nội" }
            );

            var dt = new DateTime(2024, 1, 1);

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh", Price = 89000, Description = "Câu chuyện tuổi thơ đầy cảm xúc tại một vùng quê nghèo miền Trung. Tác phẩm đã được chuyển thể thành phim điện ảnh nổi tiếng.", Image = "book1.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 2010, Stock = 50, IsFeatured = true, CreatedAt = dt },
                new Book { Id = 2, Title = "Mắt Biếc", Price = 75000, Description = "Tình yêu trong sáng, đơn phương của chàng trai miền quê dành cho cô gái cùng làng.", Image = "book2.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 1990, Stock = 40, IsFeatured = true, CreatedAt = dt },
                new Book { Id = 3, Title = "Dế Mèn Phiêu Lưu Ký", Price = 45000, Description = "Cuộc phiêu lưu đầy thú vị của chú Dế Mèn qua những miền đất lạ.", Image = "book3.jpg", AuthorId = 2, PublisherId = 2, CategoryId = 6, PublicationYear = 1941, Stock = 100, CreatedAt = dt },
                new Book { Id = 4, Title = "Đắc Nhân Tâm", Price = 68000, Description = "Nghệ thuật giao tiếp và xử thế trong cuộc sống, kinh doanh.", Image = "book4.jpg", AuthorId = 3, PublisherId = 1, CategoryId = 4, PublicationYear = 1936, Stock = 80, IsFeatured = true, SalePrice = 49000, SaleEndDate = new DateTime(2026, 12, 31), CreatedAt = dt },
                new Book { Id = 5, Title = "Cha Giàu Cha Nghèo", Price = 95000, Description = "Bài học tài chính cá nhân từ hai người cha có quan điểm khác nhau.", Image = "book5.jpg", AuthorId = 4, PublisherId = 1, CategoryId = 3, PublicationYear = 1997, Stock = 60, IsFeatured = true, CreatedAt = dt },
                new Book { Id = 6, Title = "Nhà Giả Kim", Price = 59000, Description = "Hành trình theo đuổi giấc mơ của chàng chăn cừu Santiago.", Image = "book6.jpg", AuthorId = 5, PublisherId = 5, CategoryId = 2, PublicationYear = 1988, Stock = 70, SalePrice = 39000, SaleEndDate = new DateTime(2026, 12, 31), CreatedAt = dt },
                new Book { Id = 7, Title = "Sapiens: Lược Sử Loài Người", Price = 159000, Description = "Lịch sử loài người từ thời tiền sử đến thế kỷ 21.", Image = "book7.jpg", AuthorId = 6, PublisherId = 5, CategoryId = 5, PublicationYear = 2011, Stock = 35, IsFeatured = true, CreatedAt = dt },
                new Book { Id = 8, Title = "Cánh Đồng Bất Tận", Price = 72000, Description = "Truyện ngắn đặc sắc về cuộc sống miền Tây Nam Bộ.", Image = "book8.jpg", AuthorId = 7, PublisherId = 1, CategoryId = 1, PublicationYear = 2005, Stock = 45, CreatedAt = dt },
                new Book { Id = 9, Title = "Harry Potter và Hòn Đá Phù Thủy", Price = 120000, Description = "Phần đầu tiên trong series Harry Potter huyền thoại.", Image = "book9.jpg", AuthorId = 8, PublisherId = 1, CategoryId = 2, PublicationYear = 1997, Stock = 55, IsFeatured = true, SalePrice = 89000, SaleEndDate = new DateTime(2026, 12, 31), CreatedAt = dt },
                new Book { Id = 10, Title = "Nghĩ Giàu Làm Giàu", Price = 78000, Description = "13 nguyên tắc làm giàu kinh điển từ Napoleon Hill.", Image = "book10.jpg", AuthorId = 9, PublisherId = 4, CategoryId = 3, PublicationYear = 1937, Stock = 65, CreatedAt = dt },
                new Book { Id = 11, Title = "Cho Tôi Xin Một Vé Đi Tuổi Thơ", Price = 65000, Description = "Thế giới tuổi thơ hồn nhiên qua góc nhìn của người lớn.", Image = "book11.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 2008, Stock = 42, CreatedAt = dt },
                new Book { Id = 12, Title = "Quẳng Gánh Lo Đi Và Vui Sống", Price = 72000, Description = "Bí quyết sống vui vẻ, lạc quan và chiến thắng lo lắng.", Image = "book12.jpg", AuthorId = 3, PublisherId = 1, CategoryId = 4, PublicationYear = 1948, Stock = 38, SalePrice = 52000, SaleEndDate = new DateTime(2026, 12, 31), CreatedAt = dt },
                new Book { Id = 13, Title = "21 Bài Học Cho Thế Kỷ 21", Price = 145000, Description = "Thách thức lớn nhất mà nhân loại phải đối mặt.", Image = "book13.jpg", AuthorId = 6, PublisherId = 5, CategoryId = 8, PublicationYear = 2018, Stock = 30, CreatedAt = dt },
                new Book { Id = 14, Title = "Harry Potter và Phòng Chứa Bí Mật", Price = 125000, Description = "Phần hai trong series Harry Potter.", Image = "book14.jpg", AuthorId = 8, PublisherId = 1, CategoryId = 2, PublicationYear = 1998, Stock = 48, CreatedAt = dt },
                new Book { Id = 15, Title = "Dạy Con Làm Giàu - Tập 1", Price = 85000, Description = "Bài học tài chính dành cho gia đình.", Image = "book15.jpg", AuthorId = 4, PublisherId = 1, CategoryId = 3, PublicationYear = 2000, Stock = 33, CreatedAt = dt },
                new Book { Id = 16, Title = "Tắt Đèn", Price = 35000, Description = "Bức tranh xã hội Việt Nam trước cách mạng tháng Tám.", Image = "book16.jpg", AuthorId = 2, PublisherId = 6, CategoryId = 1, PublicationYear = 1937, Stock = 90, CreatedAt = dt },
                new Book { Id = 17, Title = "Homo Deus: Lược Sử Tương Lai", Price = 155000, Description = "Tương lai loài người trong thời đại AI và công nghệ.", Image = "book17.jpg", AuthorId = 6, PublisherId = 5, CategoryId = 5, PublicationYear = 2015, Stock = 28, CreatedAt = dt },
                new Book { Id = 18, Title = "Ngồi Khóc Trên Cây", Price = 69000, Description = "Câu chuyện tình cảm đầy chất thơ của Nguyễn Nhật Ánh.", Image = "book18.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 2013, Stock = 37, CreatedAt = dt },
                new Book { Id = 19, Title = "Sông", Price = 68000, Description = "Tiểu thuyết đầy chất thơ về cuộc sống miền sông nước.", Image = "book19.jpg", AuthorId = 7, PublisherId = 1, CategoryId = 1, PublicationYear = 2012, Stock = 25, CreatedAt = dt },
                new Book { Id = 20, Title = "Lập Trình C# Căn Bản", Price = 110000, Description = "Giáo trình lập trình C# dành cho sinh viên CNTT.", Image = "book20.jpg", AuthorId = 10, PublisherId = 6, CategoryId = 7, PublicationYear = 2020, Stock = 75, CreatedAt = dt }
            );

            // Banners
            modelBuilder.Entity<Banner>().HasData(
                new Banner { Id = 1, Title = "Khám Phá Thế Giới Qua Trang Sách", Subtitle = "Hàng ngàn đầu sách đang chờ bạn khám phá với ưu đãi lên đến 50%", LinkUrl = "/Book", DisplayOrder = 1, IsActive = true, CreatedAt = dt },
                new Banner { Id = 2, Title = "Flash Sale Cuối Tuần", Subtitle = "Giảm giá sốc lên đến 40% cho các đầu sách bán chạy nhất", LinkUrl = "/Book?sortBy=bestseller", DisplayOrder = 2, IsActive = true, CreatedAt = dt },
                new Banner { Id = 3, Title = "Sách Mới Phát Hành", Subtitle = "Cập nhật những đầu sách mới nhất từ các tác giả hàng đầu", LinkUrl = "/Book?sortBy=newest", DisplayOrder = 3, IsActive = true, CreatedAt = dt }
            );

            // Coupons
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = 1, Code = "WELCOME10", DiscountType = "Percentage", DiscountValue = 10, MinOrderAmount = 100000, MaxDiscount = 50000, StartDate = dt, EndDate = new DateTime(2026, 12, 31), UsageLimit = 100, IsActive = true, Description = "Giảm 10% cho đơn hàng đầu tiên", CreatedAt = dt },
                new Coupon { Id = 2, Code = "SAVE20K", DiscountType = "Fixed", DiscountValue = 20000, MinOrderAmount = 200000, StartDate = dt, EndDate = new DateTime(2026, 12, 31), UsageLimit = 50, IsActive = true, Description = "Giảm 20.000đ cho đơn từ 200.000đ", CreatedAt = dt },
                new Coupon { Id = 3, Code = "BOOKWORM", DiscountType = "Percentage", DiscountValue = 15, MinOrderAmount = 300000, MaxDiscount = 100000, StartDate = dt, EndDate = new DateTime(2026, 12, 31), UsageLimit = 30, IsActive = true, Description = "Giảm 15% cho đơn từ 300.000đ", CreatedAt = dt }
            );

            // Reviews
            modelBuilder.Entity<Review>().HasData(
                new Review { Id = 1, BookId = 1, UserId = 2, Rating = 5, Comment = "Tuyệt vời! Đọc xong muốn quay lại tuổi thơ.", CreatedAt = dt },
                new Review { Id = 2, BookId = 2, UserId = 2, Rating = 5, Comment = "Một tình yêu trong sáng và đầy cảm xúc.", CreatedAt = dt },
                new Review { Id = 3, BookId = 4, UserId = 2, Rating = 4, Comment = "Sách rất hay, giúp cải thiện kỹ năng giao tiếp.", CreatedAt = dt },
                new Review { Id = 4, BookId = 5, UserId = 2, Rating = 5, Comment = "Bài học tài chính quý giá, ai cũng nên đọc.", CreatedAt = dt },
                new Review { Id = 5, BookId = 6, UserId = 2, Rating = 4, Comment = "Câu chuyện truyền cảm hứng, đáng đọc.", CreatedAt = dt },
                new Review { Id = 6, BookId = 7, UserId = 2, Rating = 5, Comment = "Kiến thức sâu rộng, văn phong hấp dẫn.", CreatedAt = dt },
                new Review { Id = 7, BookId = 9, UserId = 2, Rating = 5, Comment = "Harry Potter luôn là kiệt tác!", CreatedAt = dt }
            );
        }
    }
}
