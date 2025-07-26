using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface ICartItemRepository:IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(int cartId, string? includes = null);
        Task<CartItem?> GetItemByIdAsync(int id);
        Task<CartItem?> GetItemByProductAsync(int cartId, int productId);
        Task AddAsync(CartItem item);
        Task UpdateAsync(CartItem item);
        Task DeleteAsync(CartItem item);
    }
}
