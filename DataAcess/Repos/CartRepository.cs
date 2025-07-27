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
    public class CartRepository:Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context):base(context) {
        
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetAllAsync(string? includes = null)
        {
            IQueryable<Cart> query = _context.Carts;

            if (!string.IsNullOrEmpty(includes))
                query = query.Include(includes);

            return await query.ToListAsync();
        }

        public async Task<Cart?> GetByIdAsync(int id, string? includes = null)
        {
            IQueryable<Cart> query = _context.Carts;

            if (!string.IsNullOrEmpty(includes))
                query = query.Include(includes);

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart?> GetByBuyerIdAsync(string buyerId, string? includes = null)
        {
            IQueryable<Cart> query = _context.Carts;

            if (!string.IsNullOrEmpty(includes))
                query = query.Include(includes);

            return await query.FirstOrDefaultAsync(c => c.BuyerId == buyerId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
