using AutoMapper;
using DataAcess.Repos.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos
{
    public class ProductRepo : IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepo(ApplicationDbContext _db)
        {
            db = _db;
        }

        public async Task CreateProductAsync(Product product)
        {
            await db.Products.AddAsync(product);
            await db.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product p)
        {
            db.Products.Remove(p);
            await db.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProductsBySeriviceId(int seriviceId)
        {
            return await db.Products.Where(p => p.ServiceId == seriviceId).ToListAsync();
        }
        public async Task<List<Product>> GetAllProducts()
        {
            return await db.Products.ToListAsync();
        }


        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await db.Products.FindAsync(id);
        }

        public async Task UpdateProductAsync(Product product)
        {
            db.Products.Update(product);
            await db.SaveChangesAsync();
        }
    }
}
