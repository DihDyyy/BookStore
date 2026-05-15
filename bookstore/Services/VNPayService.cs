using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace bookstore.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress);
        bool ValidateCallback(IQueryCollection queryParams);
        string GetResponseCode(IQueryCollection queryParams);
        string GetTransactionId(IQueryCollection queryParams);
    }

    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _configuration;

        public VNPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress)
        {
            var tmnCode = _configuration["VNPay:TmnCode"] ?? "DEMO1234";
            var hashSecret = _configuration["VNPay:HashSecret"] ?? "DEMOSECRETKEY12345678901234567890";
            var baseUrl = _configuration["VNPay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            var returnUrl = _configuration["VNPay:ReturnUrl"] ?? "http://localhost:5100/Order/VNPayCallback";

            var vnpParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", orderId.ToString() },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            var queryString = string.Join("&", vnpParams.Select(kv =>
                $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            var signData = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
            var secureHash = HmacSha512(hashSecret, signData);

            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateCallback(IQueryCollection queryParams)
        {
            var hashSecret = _configuration["VNPay:HashSecret"] ?? "DEMOSECRETKEY12345678901234567890";
            var vnpSecureHash = queryParams["vnp_SecureHash"].ToString();

            var vnpParams = new SortedDictionary<string, string>();
            foreach (var key in queryParams.Keys)
            {
                if (key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    vnpParams[key] = queryParams[key].ToString();
                }
            }

            var signData = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
            var checkHash = HmacSha512(hashSecret, signData);

            return checkHash.Equals(vnpSecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public string GetResponseCode(IQueryCollection queryParams)
        {
            return queryParams["vnp_ResponseCode"].ToString();
        }

        public string GetTransactionId(IQueryCollection queryParams)
        {
            return queryParams["vnp_TransactionNo"].ToString();
        }

        private static string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
