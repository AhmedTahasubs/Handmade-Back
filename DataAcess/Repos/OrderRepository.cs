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
    public class OrderRepository: Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
        }
        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
