using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.Domain;
using Models.DTOs.Service;
using System.Collections.Generic;
using System.Linq;

namespace IdentityManager.Services.ControllerService
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository repo;

        public ServiceService(IServiceRepository repo)
        {
            this.repo = repo;
        }

        // ✅ تحويل Entity → DTO (للإرجاع)
        private static ServiceDto ToDto(Service s) => new ServiceDto
        {
            Id = s.Id,
            Title = s.Name,
            Description = s.Description,
            BasePrice = s.BasePrice,
            DeliveryTime = s.DeliveryTime,
            Status = s.Status,

            // معلومات إضافية من العلاقات
            SellerName = s.Seller?.FullName,
            CategoryName = s.Category?.Name,
            AvgRating = s.Reviews?.Any() == true ? s.Reviews.Average(r => r.Rating) : 0,

            SellerId = s.SellerId,
            CategoryId = s.CategoryId,
            ImageId = s.ImageId
        };

        // ✅ تحويل Create DTO → Entity
        private static Service ToEntity(CreateServiceDto dto) => new Service
        {
            Name = dto.Title,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            DeliveryTime = dto.DeliveryTime,
            Status = "active", // دايمًا عند الإنشاء
            SellerId = dto.SellerId,
            CategoryId = dto.CategoryId,
            ImageId = dto.ImageId
        };

        // ✅ تعديل Entity باستخدام Update DTO
        private static void UpdateEntity(Service existing, UpdateServiceDto dto)
        {
            existing.Name = dto.Title;
            existing.Description = dto.Description;
            existing.BasePrice = dto.BasePrice;
            existing.DeliveryTime = dto.DeliveryTime;
            existing.Status = dto.Status ?? existing.Status; // لو مش مبعوتة نخلي القديمة
            existing.CategoryId = dto.CategoryId;
            existing.ImageId = dto.ImageId;
        }

        // ✅ إنشاء خدمة جديدة
        public ServiceDto Create(CreateServiceDto dto)
        {
            var entity = ToEntity(dto);
            var added = repo.ADD(entity);
            repo.SavaChange();
            return ToDto(added);
        }

        // ✅ جلب كل الخدمات
        public IEnumerable<ServiceDto> GetAll()
        {
            var services = repo.GetAll();
            return services.Select(ToDto);
        }

        // ✅ جلب خدمة واحدة بالـ ID
        public ServiceDto GetByID(int id)
        {
            var service = repo.Getbyid(id);
            return service == null ? null : ToDto(service);
        }

        // ✅ تعديل خدمة
        public ServiceDto Update(int id, UpdateServiceDto dto)
        {
            var existing = repo.Getbyid(id);
            if (existing == null) return null;

            UpdateEntity(existing, dto);

            var updated = repo.UPDATE(existing);
            repo.SavaChange();
            return ToDto(updated);
        }

        // ✅ حذف خدمة
        public bool Delete(int id)
        {
            var deleted = repo.Delete(id);
            if (!deleted) return false;

            repo.SavaChange();
            return true;
        }
    }
}
