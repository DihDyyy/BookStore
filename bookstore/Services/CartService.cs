using bookstore.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace bookstore.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(CartItem item);
        void RemoveFromCart(int bookId);
        void UpdateQuantity(int bookId, int quantity);
        void ClearCart();
        int GetCartCount();
        decimal GetCartTotal();
    }

    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public List<CartItem> GetCart()
        {
            var cartJson = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            Session.SetString(CartSessionKey, cartJson);
        }

        public void AddToCart(CartItem item)
        {
            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.BookId == item.BookId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.BookId == bookId);
            SaveCart(cart);
        }

        public void UpdateQuantity(int bookId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.BookId == bookId);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SaveCart(cart);
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        public int GetCartCount()
        {
            return GetCart().Sum(c => c.Quantity);
        }

        public decimal GetCartTotal()
        {
            return GetCart().Sum(c => c.Total);
        }
    }
}
