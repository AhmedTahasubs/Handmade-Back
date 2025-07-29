using Models.DTOs.ServiceReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface IServiceReviewService
    {
        IEnumerable<ServiceReviewDto> GetAll();
        ServiceReviewDto GetById(int id);
        ServiceReviewDto Create(CreateServiceReviewDto dto);
        ServiceReviewDto Update(int id, UpdateServiceReviewDto dto);
        bool Delete(int id);
        IEnumerable<ServiceReviewDto> GetByServiceId(int serviceId);
    }
}
