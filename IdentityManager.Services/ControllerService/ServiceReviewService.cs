using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.Domain;
using Models.DTOs.ServiceReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService
{
    public class ServiceReviewService : IServiceReviewService
    {
        private readonly IServiceReviewRepository repo;

        public ServiceReviewService(IServiceReviewRepository repo) {
        
            this.repo=repo;
        }
        // Entity -> DTO
        private static ServiceReviewDto ToDto(ServiceReview r) => new ServiceReviewDto
        {
            Id = r.Id,
            ServiceId = r.ServiceId,
            ServiceTitle = r.Service?.Name,
            ReviewerId = r.ReviewerId,
            ReviewerName = r.Reviewer?.FullName,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        };

        // Create DTO -> Entity
        private static ServiceReview ToEntity(CreateServiceReviewDto dto) => new ServiceReview
        {
            ServiceId = dto.ServiceId,
            ReviewerId = dto.ReviewerId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        public IEnumerable<ServiceReviewDto> GetAll()
        {
            return repo.GetAll().Select(r => ToDto(r));
        }

        public ServiceReviewDto GetById(int id)
        {
            var review = repo.GetById(id);
            return review == null ? null : ToDto(review);
        }

        public ServiceReviewDto Create(CreateServiceReviewDto dto)
        {
            var entity = ToEntity(dto);
            repo.Add(entity);
            repo.SavaChange();
            return ToDto(repo.GetById(entity.Id));
        }

        public ServiceReviewDto Update(int id, UpdateServiceReviewDto dto)
        {
            var existing = repo.GetById(id);
            if (existing == null) return null;

            existing.Rating = dto.Rating;
            existing.Comment = dto.Comment;

            var updated = repo.Update(existing);
            repo.SavaChange();
            return ToDto(updated);
        }

        public bool Delete(int id)
        {
            var result = repo.Delete(id);
            if (result) repo.SavaChange();
            return result;
        }

        public IEnumerable<ServiceReviewDto> GetByServiceId(int serviceId)
        {
            return repo.GetByServiceId(serviceId).Select(r => ToDto(r));

        }
    }
}
