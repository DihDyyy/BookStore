using bookstore.Data;
using bookstore.Models;
using bookstore.Models.ViewModels;
using bookstore.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace bookstore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext context, IFileUploadService fileUploadService, IEmailService emailService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _emailService = emailService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa");
                return View(model);
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Avatar", user.Avatar ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["Success"] = $"Chào mừng {user.Name}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Check duplicate email
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng");
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "User",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Phone = model.Phone;
            await _context.SaveChangesAsync();

            // Refresh claims
            await RefreshClaims(user);

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        // POST: /Account/UploadAvatar
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (avatarFile == null || avatarFile.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file ảnh";
                return RedirectToAction("Profile");
            }

            try
            {
                // Delete old avatar
                if (!string.IsNullOrEmpty(user.Avatar))
                    _fileUploadService.DeleteFile(user.Avatar);

                user.Avatar = await _fileUploadService.UploadFileAsync(avatarFile, "avatars");
                await _context.SaveChangesAsync();
                await RefreshClaims(user);

                TempData["Success"] = "Cập nhật ảnh đại diện thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Profile");
        }

        // GET: /Account/ChangePassword
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Account/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            {
                ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không đúng");
                return View(model);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                // Don't reveal user doesn't exist
                TempData["Success"] = "Nếu email tồn tại, chúng tôi đã gửi link đặt lại mật khẩu.";
                return RedirectToAction("Login");
            }

            // Generate reset token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.PasswordResetToken = token;
            user.ResetTokenExpiry = DateTime.Now.AddHours(1);
            await _context.SaveChangesAsync();

            // Send email
            var resetLink = Url.Action("ResetPassword", "Account",
                new { token = token, email = user.Email }, Request.Scheme);
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink!);

            TempData["Success"] = "Nếu email tồn tại, chúng tôi đã gửi link đặt lại mật khẩu.";
            return RedirectToAction("Login");
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == model.Email &&
                u.PasswordResetToken == model.Token &&
                u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                ModelState.AddModelError("", "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
                return View(model);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Addresses
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Addresses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .ToListAsync();

            return View(addresses);
        }

        // POST: /Account/AddAddress
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(UserAddress address)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            address.UserId = userId;

            ModelState.Remove("User");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin không hợp lệ";
                return RedirectToAction("Addresses");
            }

            if (address.IsDefault)
            {
                var existing = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();
                foreach (var a in existing) a.IsDefault = false;
            }

            _context.UserAddresses.Add(address);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm địa chỉ thành công!";
            return RedirectToAction("Addresses");
        }

        // POST: /Account/DeleteAddress
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address != null)
            {
                _context.UserAddresses.Remove(address);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa địa chỉ thành công!";
            }

            return RedirectToAction("Addresses");
        }

        // POST: /Account/SetDefaultAddress
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var allAddresses = await _context.UserAddresses
                .Where(a => a.UserId == userId).ToListAsync();

            foreach (var a in allAddresses)
                a.IsDefault = a.Id == id;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã đặt địa chỉ mặc định!";
            return RedirectToAction("Addresses");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task RefreshClaims(User user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Avatar", user.Avatar ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) });
        }
    }
}
