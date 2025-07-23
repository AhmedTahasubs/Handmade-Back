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
    public class ServiceReviewRepository : IServiceReviewRepository
    {
        private readonly ApplicationDbContext db;

        public ServiceReviewRepository(ApplicationDbContext db ) {
            this.db = db;
        }

        public ServiceReview Add(ServiceReview serviceReview)
        {
           
            db.ServiceReviews.Add(serviceReview);
            return serviceReview;
        }

        public bool Delete(int id)
        {
           var delete = db.ServiceReviews.SingleOrDefault(s => s.Id == id);
            if (delete == null) { return false; }
            return true;
        }

        public IEnumerable<ServiceReview> GetAll()
        {
            return db.ServiceReviews.Include(s=>s.Service).Include(s=>s.Reviewer).ToList();
        }

        public ServiceReview GetById(int id)
        {
            return db.ServiceReviews.Include(s => s.Service)
                .Include(s => s.Reviewer)
                .SingleOrDefault(s => s.Id == id);
        }

        public void SavaChange()
        {
            db.SaveChanges();
        }

        public ServiceReview Update(ServiceReview serviceReview)
        {
            db.ServiceReviews.Update(serviceReview);
            return serviceReview;
        }
    }
}
