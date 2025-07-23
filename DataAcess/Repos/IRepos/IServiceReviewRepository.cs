using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IServiceReviewRepository
    {
        IEnumerable<ServiceReview> GetAll();
        ServiceReview GetById(int id);
        ServiceReview Add(ServiceReview serviceReview);
        ServiceReview Update(ServiceReview serviceReview);
        bool Delete(int id);
        void SavaChange();

    }
}
