using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();

        Task<Product?> GetProductByIdAsync(int id);
        Task<List<Product>> GetAllProductsBySeriviceId(int seriviceId);
        Task<List<Product>> GetAllProductsBySellerId(string sellerId);

        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product p);
        Task<int> SaveAsync();
        Task<Product?> UpdateProductStatusAsync(int id, string status);

    }
}
