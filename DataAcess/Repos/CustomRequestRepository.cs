using DataAcess.Repos.IRepos;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos
{
    public class CustomRequestRepository: Repository<CustomRequest>, ICustomRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<CustomRequest> GetByIdAsync(int id)
        {
            return await _context.CustomRequests.FindAsync(id);
        }
        public void Update(CustomRequest request)
        {
            _context.CustomRequests.Update(request);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
