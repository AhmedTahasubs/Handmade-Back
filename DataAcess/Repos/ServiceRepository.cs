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
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext db;
        public ServiceRepository(ApplicationDbContext db)
        {
            this.db = db;
        }
        public Service ADD(Service service)
        {
            db.Add(service);
            return service;

        }

        public IEnumerable<Service> GetAll()
        {
            return db.Services.Include(s => s.Seller)
                .Include(c => c.Category)
                .Include(s => s.Reviews)
                .ToList();
        }

        public Service Getbyid(int id)
        {
            return db.Services.Include(s => s.Seller)
                .Include(c => c.Category)
                .Include(s => s.Reviews)
                .Include(s => s.Products)
               .ThenInclude(p => p.Image)
                .FirstOrDefault(s => s.Id == id);

        }

        public bool Delete(int id)
        {
            var temp = db.Services.SingleOrDefault(s => s.Id == id);
            if (temp == null) { return false; }
            db.Services.Remove(temp);
            return true;
        }

        public Service UPDATE(Service service)
        {
            db.Update(service);
            return service;
        }

        public void SavaChange()
        {
            db.SaveChanges();
        }

        public IEnumerable<Service> GetAllBySellerId(string sellerId)
        {
            return db.Services
                .Include(s => s.Seller)
                .Include(c => c.Category)
                .Include(s => s.Reviews)
                .Where(s => s.SellerId == sellerId)
                .ToList();
        }

        public IEnumerable<Service> GetAllByCategoryId(int categoryId)
        {
            return db.Services
                .Include(s => s.Seller)
                .Include(c => c.Category)
                .Include(s => s.Reviews)
                .Where(s => s.CategoryId == categoryId)
                .ToList();
        }

        public IEnumerable<Service> GetAllByCategoryName(string categoryName)
        {
            return db.Services
                .Include(s => s.Seller)
                .Include(c => c.Category)
                .Include(s => s.Reviews)
                .Where(s => s.Category.Name == categoryName)
                .ToList();
        }
        public async Task<Service?> UpdateServiceStatusAsync(int id, string status)
        {
            var p = await db.Services.FindAsync(id);
            if (p == null)
                return null;
            p.Status = status;
            return p;
        }

        public async Task<Service?> UpdateServiceReason(int id, string status)
        {
           var s = await db.Services.FindAsync(id);
            if (s==null)
                return null;
            s.Reason = status;
            s.Status = "rejected";
            return s;
        }
    }
}
