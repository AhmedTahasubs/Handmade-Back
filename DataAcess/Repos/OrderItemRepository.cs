using DataAcess.Repos.IRepos;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<OrderItem> GetByIdAsync(int id)
        {
            return await _context.OrderItems.FindAsync(id);
        }
        public void Update(OrderItem item)
        {
            _context.OrderItems.Update(item);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
