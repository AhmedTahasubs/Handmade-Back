using Models.Domain;
using Models.DTOs.Categories;
using Models.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface IServiceService
    {
        IEnumerable<ServiceDto> GetAll();
        ServiceDto  GetByID(int id);
        ServiceDto Create(CreateServiceDto dto);
        ServiceDto Update(int id ,UpdateServiceDto dto);
        bool Delete(int id);
        IEnumerable<ServiceDto> GetAllBySellerId(string sellerId);
        IEnumerable<ServiceDto> GetAllByCategoryId(int categoryId);

    }
}
