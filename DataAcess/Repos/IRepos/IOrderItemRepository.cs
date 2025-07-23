using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IOrderItemRepository: IRepository<OrderItem>
    {
        Task<OrderItem> GetByIdAsync(int id);
        void Update(OrderItem item);
        Task<bool> SaveChangesAsync();
    }
}
