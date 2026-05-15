using System.Net;
using System.Net.Mail;

namespace bookstore.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task SendOrderConfirmationAsync(string toEmail, string orderCode, decimal totalAmount);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"] ?? "";
                var smtpPass = _configuration["Email:SmtpPass"] ?? "";
                var fromEmail = _configuration["Email:FromEmail"] ?? smtpUser;
                var fromName = _configuration["Email:FromName"] ?? "BookStore";

                if (string.IsNullOrEmpty(smtpUser))
                {
                    _logger.LogWarning("Email SMTP chưa được cấu hình. Email tới {Email} không được gửi.", toEmail);
                    return;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var subject = "BookStore - Đặt lại mật khẩu";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; border-radius: 10px 10px 0 0; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>📚 BookStore</h1>
                    </div>
                    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px;'>
                        <h2 style='color: #333;'>Đặt lại mật khẩu</h2>
                        <p style='color: #666;'>Bạn đã yêu cầu đặt lại mật khẩu. Nhấn vào nút bên dưới để tạo mật khẩu mới:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' style='background: linear-gradient(135deg, #667eea, #764ba2); color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Đặt lại mật khẩu</a>
                        </div>
                        <p style='color: #999; font-size: 12px;'>Link này sẽ hết hạn sau 1 giờ. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderConfirmationAsync(string toEmail, string orderCode, decimal totalAmount)
        {
            var subject = $"BookStore - Xác nhận đơn hàng #{orderCode}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; border-radius: 10px 10px 0 0; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>📚 BookStore</h1>
                    </div>
                    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px;'>
                        <h2 style='color: #333;'>Đặt hàng thành công! 🎉</h2>
                        <p style='color: #666;'>Mã đơn hàng: <strong>#{orderCode}</strong></p>
                        <p style='color: #666;'>Tổng tiền: <strong>{totalAmount:N0}đ</strong></p>
                        <p style='color: #666;'>Cảm ơn bạn đã mua sách tại BookStore!</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
