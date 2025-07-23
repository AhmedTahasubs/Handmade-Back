using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IOrderRepository: IRepository<Order>
    {
        Task<Order> GetByIdAsync(int id);
        void Update(Order order);
        Task<bool> SaveChangesAsync();
    }
}
