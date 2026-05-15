using bookstore.Data;
using bookstore.Models;
using Microsoft.EntityFrameworkCore;

namespace bookstore.Services
{
    public interface ICouponService
    {
        Task<(bool IsValid, string Message, decimal DiscountAmount)> ValidateCoupon(string code, decimal orderTotal);
        Task IncrementUsage(string code);
    }

    public class CouponService : ICouponService
    {
        private readonly ApplicationDbContext _context;

        public CouponService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, string Message, decimal DiscountAmount)> ValidateCoupon(string code, decimal orderTotal)
        {
            if (string.IsNullOrWhiteSpace(code))
                return (false, "Vui lòng nhập mã coupon", 0);

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code.Trim().ToUpper());

            if (coupon == null)
                return (false, "Mã coupon không tồn tại", 0);

            if (!coupon.IsActive)
                return (false, "Mã coupon đã bị vô hiệu hóa", 0);

            if (DateTime.Now < coupon.StartDate)
                return (false, "Mã coupon chưa có hiệu lực", 0);

            if (DateTime.Now > coupon.EndDate)
                return (false, "Mã coupon đã hết hạn", 0);

            if (coupon.UsedCount >= coupon.UsageLimit)
                return (false, "Mã coupon đã hết lượt sử dụng", 0);

            if (orderTotal < coupon.MinOrderAmount)
                return (false, $"Đơn hàng tối thiểu {coupon.MinOrderAmount:N0}đ để sử dụng mã này", 0);

            decimal discount;
            if (coupon.DiscountType == "Percentage")
            {
                discount = orderTotal * coupon.DiscountValue / 100;
                if (coupon.MaxDiscount.HasValue && discount > coupon.MaxDiscount.Value)
                    discount = coupon.MaxDiscount.Value;
            }
            else // Fixed
            {
                discount = coupon.DiscountValue;
            }

            // Ensure discount doesn't exceed order total
            if (discount > orderTotal)
                discount = orderTotal;

            return (true, $"Áp dụng mã {coupon.Code} thành công! Giảm {discount:N0}đ", discount);
        }

        public async Task IncrementUsage(string code)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code.Trim().ToUpper());

            if (coupon != null)
            {
                coupon.UsedCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}
