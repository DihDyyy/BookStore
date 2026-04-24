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
                new User { Id = 2, Name = "Nguyễn Văn A", Email = "user@bookstore.com", Password = hash, Role = "User", CreatedAt = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Văn học Việt Nam" },
                new Category { Id = 2, Name = "Văn học nước ngoài" },
                new Category { Id = 3, Name = "Kinh tế" },
                new Category { Id = 4, Name = "Kỹ năng sống" },
                new Category { Id = 5, Name = "Khoa học công nghệ" },
                new Category { Id = 6, Name = "Thiếu nhi" },
                new Category { Id = 7, Name = "Giáo trình" },
                new Category { Id = 8, Name = "Tâm lý - Triết học" }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "Nguyễn Nhật Ánh" },
                new Author { Id = 2, Name = "Tô Hoài" },
                new Author { Id = 3, Name = "Dale Carnegie" },
                new Author { Id = 4, Name = "Robert Kiyosaki" },
                new Author { Id = 5, Name = "Paulo Coelho" },
                new Author { Id = 6, Name = "Yuval Noah Harari" },
                new Author { Id = 7, Name = "Nguyễn Ngọc Tư" },
                new Author { Id = 8, Name = "J.K. Rowling" },
                new Author { Id = 9, Name = "Napoleon Hill" },
                new Author { Id = 10, Name = "Trần Đắc Trung" }
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
                new Book { Id = 1, Title = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh", Price = 89000, Description = "Câu chuyện tuổi thơ đầy cảm xúc.", Image = "book1.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 2010, Stock = 50, CreatedAt = dt },
                new Book { Id = 2, Title = "Mắt Biếc", Price = 75000, Description = "Tình yêu trong sáng, đơn phương.", Image = "book2.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 1990, Stock = 40, CreatedAt = dt },
                new Book { Id = 3, Title = "Dế Mèn Phiêu Lưu Ký", Price = 45000, Description = "Cuộc phiêu lưu của chú Dế Mèn.", Image = "book3.jpg", AuthorId = 2, PublisherId = 2, CategoryId = 6, PublicationYear = 1941, Stock = 100, CreatedAt = dt },
                new Book { Id = 4, Title = "Đắc Nhân Tâm", Price = 68000, Description = "Nghệ thuật giao tiếp và xử thế.", Image = "book4.jpg", AuthorId = 3, PublisherId = 1, CategoryId = 4, PublicationYear = 1936, Stock = 80, CreatedAt = dt },
                new Book { Id = 5, Title = "Cha Giàu Cha Nghèo", Price = 95000, Description = "Bài học tài chính cá nhân.", Image = "book5.jpg", AuthorId = 4, PublisherId = 1, CategoryId = 3, PublicationYear = 1997, Stock = 60, CreatedAt = dt },
                new Book { Id = 6, Title = "Nhà Giả Kim", Price = 59000, Description = "Hành trình theo đuổi giấc mơ.", Image = "book6.jpg", AuthorId = 5, PublisherId = 5, CategoryId = 2, PublicationYear = 1988, Stock = 70, CreatedAt = dt },
                new Book { Id = 7, Title = "Sapiens: Lược Sử Loài Người", Price = 159000, Description = "Lịch sử loài người từ tiền sử.", Image = "book7.jpg", AuthorId = 6, PublisherId = 5, CategoryId = 5, PublicationYear = 2011, Stock = 35, CreatedAt = dt },
                new Book { Id = 8, Title = "Cánh Đồng Bất Tận", Price = 72000, Description = "Truyện ngắn miền Tây Nam Bộ.", Image = "book8.jpg", AuthorId = 7, PublisherId = 1, CategoryId = 1, PublicationYear = 2005, Stock = 45, CreatedAt = dt },
                new Book { Id = 9, Title = "Harry Potter và Hòn Đá Phù Thủy", Price = 120000, Description = "Phần đầu series Harry Potter.", Image = "book9.jpg", AuthorId = 8, PublisherId = 1, CategoryId = 2, PublicationYear = 1997, Stock = 55, CreatedAt = dt },
                new Book { Id = 10, Title = "Nghĩ Giàu Làm Giàu", Price = 78000, Description = "13 nguyên tắc làm giàu kinh điển.", Image = "book10.jpg", AuthorId = 9, PublisherId = 4, CategoryId = 3, PublicationYear = 1937, Stock = 65, CreatedAt = dt },
                new Book { Id = 11, Title = "Cho Tôi Xin Một Vé Đi Tuổi Thơ", Price = 65000, Description = "Thế giới tuổi thơ hồn nhiên.", Image = "book11.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 2008, Stock = 42, CreatedAt = dt },
                new Book { Id = 12, Title = "Quẳng Gánh Lo Đi Và Vui Sống", Price = 72000, Description = "Bí quyết sống vui vẻ, lạc quan.", Image = "book12.jpg", AuthorId = 3, PublisherId = 1, CategoryId = 4, PublicationYear = 1948, Stock = 38, CreatedAt = dt },
                new Book { Id = 13, Title = "21 Bài Học Cho Thế Kỷ 21", Price = 145000, Description = "Thách thức của thế kỷ 21.", Image = "book13.jpg", AuthorId = 6, PublisherId = 5, CategoryId = 8, PublicationYear = 2018, Stock = 30, CreatedAt = dt },
                new Book { Id = 14, Title = "Harry Potter và Phòng Chứa Bí Mật", Price = 125000, Description = "Phần hai series Harry Potter.", Image = "book14.jpg", AuthorId = 8, PublisherId = 1, CategoryId = 2, PublicationYear = 1998, Stock = 48, CreatedAt = dt },
                new Book { Id = 15, Title = "Dạy Con Làm Giàu - Tập 1", Price = 85000, Description = "Bài học tài chính cho gia đình.", Image = "book15.jpg", AuthorId = 4, PublisherId = 1, CategoryId = 3, PublicationYear = 2000, Stock = 33, CreatedAt = dt },
                new Book { Id = 16, Title = "Tắt Đèn", Price = 35000, Description = "Xã hội Việt Nam trước cách mạng.", Image = "book16.jpg", AuthorId = 2, PublisherId = 6, CategoryId = 1, PublicationYear = 1937, Stock = 90, CreatedAt = dt },
                new Book { Id = 17, Title = "Homo Deus: Lược Sử Tương Lai", Price = 155000, Description = "Tương lai loài người với AI.", Image = "book17.jpg", AuthorId = 6, PublisherId = 5, CategoryId = 5, PublicationYear = 2015, Stock = 28, CreatedAt = dt },
                new Book { Id = 18, Title = "Ngồi Khóc Trên Cây", Price = 69000, Description = "Câu chuyện tình cảm đầy chất thơ.", Image = "book18.jpg", AuthorId = 1, PublisherId = 1, CategoryId = 1, PublicationYear = 2013, Stock = 37, CreatedAt = dt },
                new Book { Id = 19, Title = "Sông", Price = 68000, Description = "Tiểu thuyết đầy chất thơ.", Image = "book19.jpg", AuthorId = 7, PublisherId = 1, CategoryId = 1, PublicationYear = 2012, Stock = 25, CreatedAt = dt },
                new Book { Id = 20, Title = "Lập Trình C# Căn Bản", Price = 110000, Description = "Giáo trình lập trình C# cho SV.", Image = "book20.jpg", AuthorId = 10, PublisherId = 6, CategoryId = 7, PublicationYear = 2020, Stock = 75, CreatedAt = dt }
            );
        }
    }
}
