using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Http;
using Models.Domain;
using Models.DTOs.Service;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityManager.Services.ControllerService
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceService(IServiceRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            this.repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ✅ تحويل Entity → DTO
        private static ServiceDto ToDto(Service s) => new ServiceDto
        {
            Id = s.Id,
            Title = s.Name,
            Description = s.Description,
            BasePrice = s.BasePrice,
            DeliveryTime = s.DeliveryTime,
            Status = s.Status,
            SellerName = s.Seller?.FullName,
            CategoryName = s.Category?.Name,
            AvgRating = s.Reviews?.Any() == true ? s.Reviews.Average(r => r.Rating) : 0,
            SellerId = s.SellerId,
            CategoryId = s.CategoryId,
            ImageId = s.ImageId
        };

        // ✅ إنشاء خدمة جديدة
        public ServiceDto Create(CreateServiceDto dto)
        {
            // نجيب الـ SellerId من الـ Claims
            var sellerId = GetCurrentUserId();
            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException("User is not authenticated!");

            var entity = new Service
            {
                Name = dto.Title,
                Description = dto.Description,
                BasePrice = dto.BasePrice,
                DeliveryTime = dto.DeliveryTime,
                Status = "active", // أول ما تتعمل تبقى Active
                SellerId = sellerId,  // ✅ أخدناها من الـ Claims
                CategoryId = dto.CategoryId,
                ImageId = dto.ImageId
            };

            var added = repo.ADD(entity);
            repo.SavaChange();
            return ToDto(added);
        }

        // ✅ تعديل خدمة
        public ServiceDto Update(int id, UpdateServiceDto dto)
        {
            var existing = repo.Getbyid(id);
            if (existing == null) return null;

            // 🛑 نتحقق إن اللي بيعدل هو نفس صاحب الخدمة
            var sellerId = GetCurrentUserId();
            if (existing.SellerId != sellerId)
                throw new UnauthorizedAccessException("You cannot edit someone else's service!");

            // نحدّث البيانات
            existing.Name = dto.Title;
            existing.Description = dto.Description;
            existing.BasePrice = dto.BasePrice;
            existing.DeliveryTime = dto.DeliveryTime;
            existing.Status = dto.Status ?? existing.Status;
            existing.CategoryId = dto.CategoryId;
            existing.ImageId = dto.ImageId;

            var updated = repo.UPDATE(existing);
            repo.SavaChange();
            return ToDto(updated);
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

        // ✅ حذف خدمة
        public bool Delete(int id)
        {
            var existing = repo.Getbyid(id);
            if (existing == null) return false;

            var sellerId = GetCurrentUserId();
            if (existing.SellerId != sellerId)
                throw new UnauthorizedAccessException("You cannot delete someone else's service!");

            var deleted = repo.Delete(id);
            if (!deleted) return false;

            repo.SavaChange();
            return true;
        }

        // ✅ جلب كل خدمات Seller معيّن
        public IEnumerable<ServiceDto> GetAllBySellerId(string sellerId)
        {
            var services = repo.GetAllBySellerId(sellerId);
            return services.Select(ToDto);
        }

        // ✅ جلب كل خدمات Category معيّن
        public IEnumerable<ServiceDto> GetAllByCategoryId(int categoryId)
        {
            var services = repo.GetAllByCategoryId(categoryId);
            return services.Select(ToDto);
        }
    }
}
