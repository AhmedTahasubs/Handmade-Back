using DataAcess;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using System.Security.Claims;

namespace Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Get or create a cart for the current customer
        public async Task<ShoppingCart> GetOrCreateCartAsync(string customerId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    CustomerId = customerId
                };

                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // 2. Add product to cart
        public async Task AddProductAsync(string customerId, int productId, int quantity, decimal unitPrice)
        {
            var cart = await GetOrCreateCartAsync(customerId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            await _context.SaveChangesAsync();
        }

        // 3. Update quantity
        public async Task UpdateQuantityAsync(string customerId, int itemId, int newQuantity)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
                throw new Exception("Cart item not found");

            item.Quantity = newQuantity;
            await _context.SaveChangesAsync();
        }

        // 4. Remove item
        public async Task RemoveItemAsync(string customerId, int itemId)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // 5. Clear all items
        public async Task ClearCartAsync(string customerId)
        {
            var cart = await GetOrCreateCartAsync(customerId);

            _context.ShoppingCartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }

        // 6. Get cart with items
        public async Task<ShoppingCart?> GetCartAsync(string customerId)
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Image)
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

    }
}
