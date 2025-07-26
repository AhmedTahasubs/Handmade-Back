using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Domain;
namespace DataAcess.Repos.IRepos
{
    public interface ICartRepository:IRepository<Cart>
    {
        
        Task<Cart?> GetByIdAsync(int id, string? includes = null);
        Task<Cart?> GetByBuyerIdAsync(string buyerId, string? includes = null);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Cart cart);
    }
}
