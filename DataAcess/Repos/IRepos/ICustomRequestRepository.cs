using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface ICustomRequestRepository: IRepository<CustomRequest>
    {
        Task<CustomRequest> GetByIdAsync(int id);
        void Update(CustomRequest request);
        Task<bool> SaveChangesAsync();

    }
}
