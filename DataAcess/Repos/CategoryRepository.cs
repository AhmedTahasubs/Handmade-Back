using AutoMapper;
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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> GetAllAsync() => await _db.Categories.Where(c => !c.IsDeleted).Include(a=>a.Services).ToListAsync();

        public async Task<Category> GetByIdAsync(int id) => await _db.Categories.Include(s=>s.Services).FirstOrDefaultAsync(c=> c.Id==id);

        public async Task<Category> AddAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return false;
            category.IsDeleted = !category.IsDeleted;
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
            return true;
        }

        public IEnumerable<Category> SearchByName(string name)
        {
            var searched = _db.Categories.Where(c=>c.Name.Contains(name)).ToList();
            return searched;
        }
    }


}
