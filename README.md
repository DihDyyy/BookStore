# 📚 Website Bán Sách - BookStore

Website bán sách trực tuyến được xây dựng bằng **ASP.NET Core MVC** với đầy đủ chức năng quản lý sách, giỏ hàng, đặt hàng và trang quản trị admin.

---

## 📋 Mục lục

- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt và chạy](#-cài-đặt-và-chạy)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [Chức năng chi tiết](#-chức-năng-chi-tiết)
- [Tài khoản mặc định](#-tài-khoản-mặc-định)
- [Cơ sở dữ liệu](#-cơ-sở-dữ-liệu)
- [Hướng dẫn sử dụng](#-hướng-dẫn-sử-dụng)
- [Xử lý lỗi thường gặp](#-xử-lý-lỗi-thường-gặp)

---

## 🛠 Công nghệ sử dụng

| Công nghệ | Phiên bản | Mô tả |
|-----------|-----------|-------|
| ASP.NET Core MVC | .NET 9 | Framework backend chính |
| C# | 12 | Ngôn ngữ lập trình |
| Entity Framework Core | 9.0.4 | ORM - truy vấn database |
| SQL Server | 2014+ / 2022 | Cơ sở dữ liệu |
| Bootstrap | 5.3 | Framework CSS responsive |
| Font Awesome | 6.5 | Thư viện icon |
| Chart.js | 4.x | Biểu đồ doanh thu (Admin) |
| jQuery | 3.7 | Thư viện JavaScript |
| BCrypt.Net | 4.1 | Mã hóa mật khẩu |

---

## 💻 Yêu cầu hệ thống

1. **.NET SDK 9.0** trở lên  
   - Tải tại: https://dotnet.microsoft.com/download/dotnet/9.0
   - Kiểm tra: `dotnet --version`

2. **SQL Server** (một trong các phiên bản sau):
   - SQL Server 2014 trở lên
   - SQL Server Express (miễn phí)
   - SQL Server 2022

3. **SQL Server Management Studio (SSMS)** (tùy chọn, để quản lý DB trực quan)

4. **IDE** (tùy chọn):
   - Visual Studio 2022
   - Visual Studio Code
   - JetBrains Rider

---

## 🚀 Cài đặt và chạy

### Bước 1: Clone / Tải project

```
Đặt project vào thư mục, ví dụ: E:\WEB\bookstore
```

### Bước 2: Cấu hình Connection String

Mở file `appsettings.json` và sửa Connection String phù hợp với SQL Server của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=BookStoreApp;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

**Các trường hợp phổ biến:**

| SQL Server | Connection String (phần Server) |
|-----------|-------------------------------|
| SQL Server Express | `Server=.\\SQLEXPRESS` |
| SQL Server mặc định | `Server=.` hoặc `Server=localhost` |
| SQL Server với tên máy | `Server=TEN_MAY\\SQLEXPRESS` |
| SQL Server với port | `Server=localhost,1433` |

> ⚠️ **Lưu ý**: Trong file JSON, dấu `\` phải viết thành `\\`

### Bước 3: Cài đặt EF Core Tools (chỉ lần đầu)

```bash
dotnet tool install --global dotnet-ef
```

### Bước 4: Tạo Database

```bash
cd E:\WEB\bookstore
dotnet ef database update
```

Lệnh này sẽ tự động:
- Tạo database `BookStoreApp` trên SQL Server
- Tạo tất cả 8 bảng dữ liệu
- Thêm dữ liệu mẫu (20 sách, 8 danh mục, 10 tác giả, 7 NXB, 2 tài khoản)

### Bước 5: Chạy ứng dụng

```bash
dotnet build
$env:DOTNET_EnableWriteXorExecute = "0"; dotnet bin\Debug\net9.0\bookstore.dll --urls "http://localhost:5100"
```

> ⚠️ **Không dùng** `dotnet run` vì có thể gặp lỗi CLR assertion trên một số máy.

### Bước 6: Mở trình duyệt

Truy cập: **http://localhost:5100**

---

## 📁 Cấu trúc thư mục

```
bookstore/
├── Controllers/                    # Xử lý logic nghiệp vụ
│   ├── HomeController.cs           # Trang chủ
│   ├── BookController.cs           # Danh sách & chi tiết sách
│   ├── CartController.cs           # Giỏ hàng
│   ├── OrderController.cs          # Đặt hàng & lịch sử
│   ├── AccountController.cs        # Đăng nhập / Đăng ký
│   ├── WishlistController.cs       # Sách yêu thích
│   ├── AdminDashboardController.cs # Dashboard admin
│   ├── AdminBookController.cs      # CRUD sách (Admin)
│   ├── AdminCategoryController.cs  # CRUD danh mục (Admin)
│   ├── AdminAuthorController.cs    # CRUD tác giả (Admin)
│   ├── AdminPublisherController.cs # CRUD nhà xuất bản (Admin)
│   ├── AdminOrderController.cs     # Quản lý đơn hàng (Admin)
│   └── AdminUserController.cs      # Quản lý người dùng (Admin)
│
├── Models/                         # Định nghĩa dữ liệu
│   ├── User.cs                     # Người dùng
│   ├── Category.cs                 # Danh mục sách
│   ├── Author.cs                   # Tác giả
│   ├── Publisher.cs                # Nhà xuất bản
│   ├── Book.cs                     # Sách
│   ├── Order.cs                    # Đơn hàng
│   ├── OrderDetail.cs              # Chi tiết đơn hàng
│   ├── Wishlist.cs                 # Sách yêu thích
│   └── ViewModels/                 # Dữ liệu cho giao diện
│       ├── LoginViewModel.cs
│       ├── RegisterViewModel.cs
│       ├── CheckoutViewModel.cs
│       ├── BookFilterViewModel.cs
│       ├── DashboardViewModel.cs
│       └── HomeViewModel.cs
│
├── Views/                          # Giao diện (Razor Views)
│   ├── Shared/
│   │   ├── _Layout.cshtml          # Layout trang người dùng
│   │   ├── _AdminLayout.cshtml     # Layout trang admin
│   │   ├── _BookCard.cshtml        # Component card sách
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   ├── Home/Index.cshtml           # Trang chủ
│   ├── Book/                       # Trang sách
│   ├── Cart/                       # Trang giỏ hàng
│   ├── Order/                      # Trang đơn hàng
│   ├── Account/                    # Đăng nhập/Đăng ký
│   ├── Wishlist/                   # Sách yêu thích
│   └── Admin/                      # Trang quản trị
│       ├── Dashboard.cshtml
│       ├── Book/
│       ├── Category/
│       ├── Author/
│       ├── Publisher/
│       ├── Order/
│       └── User/
│
├── Data/
│   ├── ApplicationDbContext.cs     # Cấu hình Entity Framework
│   └── DbSeeder.cs                # Dữ liệu mẫu (Seed Data)
│
├── Services/
│   ├── CartService.cs              # Xử lý giỏ hàng (Session)
│   └── FileUploadService.cs        # Upload ảnh bìa sách
│
├── wwwroot/                        # File tĩnh
│   ├── css/site.css                # CSS tùy chỉnh
│   ├── js/site.js                  # JavaScript tùy chỉnh
│   └── images/books/              # Ảnh bìa sách
│
├── Migrations/                     # EF Core Migrations
├── Program.cs                      # Cấu hình ứng dụng
├── appsettings.json               # Cấu hình kết nối DB
├── global.json                    # Cấu hình SDK
└── bookstore.csproj               # File project
```

---

## ✨ Chức năng chi tiết

### 🛒 Trang người dùng (User)

| Chức năng | Mô tả |
|-----------|-------|
| **Trang chủ** | Banner, sách mới, sách bán chạy, danh mục sách |
| **Danh sách sách** | Hiển thị tất cả sách với bộ lọc và phân trang (12 sách/trang) |
| **Bộ lọc sách** | Lọc theo danh mục, tác giả, NXB, khoảng giá, sắp xếp |
| **Tìm kiếm** | Tìm kiếm Ajax real-time theo tên sách/tác giả |
| **Chi tiết sách** | Ảnh bìa, thông tin, mô tả, sách cùng thể loại |
| **Giỏ hàng** | Thêm/xóa/cập nhật số lượng, tính tổng tiền |
| **Thanh toán** | Nhập thông tin giao hàng, xác nhận đơn hàng |
| **Lịch sử đơn hàng** | Xem danh sách và chi tiết các đơn hàng đã đặt |
| **Đăng ký / Đăng nhập** | Tạo tài khoản, đăng nhập với email/mật khẩu |
| **Sách yêu thích** | Thêm/xóa sách khỏi danh sách yêu thích |

### 👨‍💼 Trang quản trị (Admin)

| Chức năng | Mô tả |
|-----------|-------|
| **Dashboard** | Thống kê tổng quan (sách, đơn hàng, doanh thu, người dùng) |
| **Biểu đồ doanh thu** | Chart.js hiển thị doanh thu theo tháng |
| **Quản lý sách** | Thêm/sửa/xóa sách, upload ảnh bìa |
| **Quản lý danh mục** | CRUD danh mục sách |
| **Quản lý tác giả** | CRUD tác giả (tên, tiểu sử) |
| **Quản lý NXB** | CRUD nhà xuất bản |
| **Quản lý đơn hàng** | Xem chi tiết, cập nhật trạng thái (Chờ xử lý → Đang giao → Đã giao / Đã hủy) |
| **Quản lý người dùng** | Xem danh sách, khóa/mở khóa tài khoản |

### 🔒 Bảo mật

- Mật khẩu mã hóa bằng **BCrypt**
- Cookie-based Authentication
- Phân quyền Admin/User (Role-based Authorization)
- Anti-Forgery Token chống CSRF
- Model Validation (server-side)

---

## 🔑 Tài khoản mặc định

| Vai trò | Email | Mật khẩu |
|---------|-------|----------|
| **Admin** | `admin@bookstore.com` | `Admin@123` |
| **User** | `user@bookstore.com` | `Admin@123` |

- **Admin**: Truy cập tất cả trang + trang quản trị `/Admin/Dashboard`
- **User**: Mua sách, giỏ hàng, đặt hàng, wishlist

---

## 🗄 Cơ sở dữ liệu

### Sơ đồ các bảng

```
Users (Người dùng)
├── Id, Name, Email, Password, Role, IsLocked, CreatedAt
│
Categories (Danh mục)          Authors (Tác giả)         Publishers (NXB)
├── Id, Name                   ├── Id, Name, Bio          ├── Id, Name
│                              │                          │
└───────────┬──────────────────┴──────────────────────────┘
            │
      Books (Sách)
      ├── Id, Title, Price, Description, Image
      ├── Stock, PublicationYear, CreatedAt
      ├── CategoryId (FK), AuthorId (FK), PublisherId (FK)
      │
      ├── Wishlists ──── UserId (FK), BookId (FK)
      │
      └── Orders (Đơn hàng)
          ├── Id, FullName, Phone, Address, Note
          ├── TotalPrice, Status, CreatedAt, UserId (FK)
          │
          └── OrderDetails (Chi tiết đơn)
              ├── Id, OrderId (FK), BookId (FK)
              ├── Quantity, Price
```

### Dữ liệu mẫu (Seed Data)

| Bảng | Số lượng | Ví dụ |
|------|---------|-------|
| Users | 2 | Admin, Nguyễn Văn A |
| Categories | 8 | Văn học VN, Kinh tế, Kỹ năng sống... |
| Authors | 10 | Nguyễn Nhật Ánh, Dale Carnegie... |
| Publishers | 7 | NXB Trẻ, NXB Kim Đồng... |
| Books | 20 | Tôi Thấy Hoa Vàng Trên Cỏ Xanh, Đắc Nhân Tâm... |

---

## 📖 Hướng dẫn sử dụng

### Người dùng (User)

1. Truy cập http://localhost:5100
2. Đăng ký tài khoản mới hoặc đăng nhập
3. Duyệt sách trên trang chủ hoặc trang danh sách sách
4. Sử dụng bộ lọc (danh mục, giá, tác giả) để tìm sách
5. Nhấn **"Thêm giỏ hàng"** để thêm sách vào giỏ
6. Vào giỏ hàng → Điều chỉnh số lượng → Nhấn **"Thanh toán"**
7. Điền thông tin giao hàng → **"Đặt hàng"**
8. Xem lịch sử đơn hàng trong menu

### Quản trị viên (Admin)

1. Đăng nhập bằng tài khoản admin
2. Truy cập `/Admin/Dashboard` (hoặc nhấn link Admin trên navbar)
3. Xem thống kê tổng quan trên Dashboard
4. Quản lý sách: Thêm/sửa/xóa, upload ảnh bìa
5. Quản lý danh mục, tác giả, NXB
6. Xem đơn hàng → Cập nhật trạng thái
7. Quản lý người dùng → Khóa/mở khóa tài khoản

### Upload ảnh bìa sách

- Vào **Admin > Quản lý sách > Thêm/Sửa sách**
- Chọn file ảnh (JPG, PNG, GIF)
- Ảnh sẽ được lưu tại `wwwroot/images/books/`

---

## ❗ Xử lý lỗi thường gặp

### 1. Lỗi CLR Assert failure khi `dotnet run`

```
CLR: Assert failure: !AreShadowStacksEnabled() || UseSpecialUserModeApc()
```

**Giải pháp:** Không dùng `dotnet run`, thay bằng:

```bash
dotnet build
$env:DOTNET_EnableWriteXorExecute = "0"; dotnet bin\Debug\net9.0\bookstore.dll --urls "http://localhost:5100"
```

### 2. Không kết nối được SQL Server

```
A network-related or instance-specific error occurred
```

**Giải pháp:**
- Kiểm tra SQL Server đang chạy (mở Services → SQL Server)
- Kiểm tra tên instance: mở SSMS xem tên server chính xác
- Sửa `appsettings.json` cho đúng (nhớ escape `\\`)

### 3. Lỗi "There is already an object named 'Users'"

**Nguyên nhân:** Database `model` của SQL Server chứa bảng từ project khác.

**Giải pháp:**
```sql
-- Mở SSMS, chọn database "model", chạy:
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) 
    + '].[' + OBJECT_NAME(parent_object_id) 
    + '] DROP CONSTRAINT [' + name + '];' 
FROM sys.foreign_keys;
EXEC sp_executesql @sql;

SET @sql = N'';
SELECT @sql += 'DROP TABLE [dbo].[' + name + '];' FROM sys.tables;
EXEC sp_executesql @sql;
```
Sau đó chạy lại `dotnet ef database update`.

### 4. Lỗi Migration

```bash
# Xóa migration cũ và tạo lại
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Lỗi "dotnet-ef does not exist"

```bash
dotnet tool install --global dotnet-ef
```

### 6. Muốn reset database (xóa hết dữ liệu)

```bash
dotnet ef database drop --force
dotnet ef database update
```

---

## 📐 Kiến trúc MVC

```
Request → Controller → Model/Service → Database
                ↓
            View (Razor) → Response (HTML)
```

- **Model**: Định nghĩa cấu trúc dữ liệu, validation
- **View**: Giao diện Razor (.cshtml) + Bootstrap 5
- **Controller**: Xử lý logic, điều hướng, trả về View

---

## 📝 Ghi chú thêm

- Project sử dụng mô hình **Code-First**: thay đổi Model → chạy Migration → cập nhật DB
- Giỏ hàng lưu trong **Session** (mất khi đóng trình duyệt)
- Authentication sử dụng **Cookie-based** (không dùng ASP.NET Identity)
- Ảnh bìa sách mặc định là placeholder icon nếu chưa upload

---

> **Tác giả**: Đồ án học phần - ASP.NET Core MVC  
> **Phiên bản**: 1.0  
> **Ngày tạo**: 04/2026
