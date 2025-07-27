using DataAcess.Repos.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos
{
    public class CartItemRepository:Repository<CartItem>,ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context):base(context) {
        
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(int cartId, string? includes = null)
        {
            IQueryable<CartItem> query = _context.CartItems.Where(i => i.CartId == cartId);

            if (!string.IsNullOrEmpty(includes))
                query = query.Include(includes);

            return await query.ToListAsync();
        }

        public async Task<CartItem?> GetItemByIdAsync(int id)
        {
            return await _context.CartItems.FindAsync(id);
        }

        public async Task<CartItem?> GetItemByProductAsync(int cartId, int productId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);
        }

        public async Task AddAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
