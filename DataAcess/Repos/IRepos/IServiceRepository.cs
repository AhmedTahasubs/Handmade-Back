using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IServiceRepository
    {
       

        IEnumerable<Service> GetAll();
        Service Getbyid(int id);
        Service ADD(Service service);
        Service UPDATE(Service service);
        bool Delete(int id);
        void SavaChange();
        IEnumerable<Service> GetAllBySellerId(string sellerId);
        IEnumerable<Service> GetAllByCategoryId(int categoryId);
        IEnumerable<Service> GetAllByCategoryName(string categoryName);
    }
}
