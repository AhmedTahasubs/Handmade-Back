using DataAcess.Repos;
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
        private readonly IServiceRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ImageRepository _imageRepo;

        public ServiceService(IServiceRepository repo, IHttpContextAccessor httpContextAccessor, ImageRepository imageRepo)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
            _imageRepo = imageRepo;
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ✅ تحويل Entity → DTO مع ImageUrl
        private ServiceDto ToDto(Service s) => new ServiceDto
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
            Products = s.Products ?? new List<Product>(), // لو مفيش منتجات يبقى قائمة فاضية
            ImageUrl = s.ImageId.HasValue ? _imageRepo.GetImageUrl(s.ImageId.Value) : null
        };

        // ✅ إنشاء خدمة جديدة
        public ServiceDto Create(CreateServiceDto dto)
        {
            // نجيب الـ SellerId من الـ Claims
            var sellerId = GetCurrentUserId();
            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException("User is not authenticated!");

            int? imageId = null;

            // ✅ لو فيه صورة ارفعها
            if (dto.File != null)
            {
                var img = new Image
                {
                    FileName = Path.GetFileNameWithoutExtension(dto.File.FileName),
                    FileExtension = Path.GetExtension(dto.File.FileName),
                    FileSize = dto.File.Length,
                    File = dto.File
                };

                var savedImage = _imageRepo.Upload(img).Result;
                imageId = savedImage.Id;
            }

            var entity = new Service
            {
                Name = dto.Title,
                Description = dto.Description,
                BasePrice = dto.BasePrice,
                DeliveryTime = dto.DeliveryTime,
                Status = "active", // أول ما تتعمل تبقى Active
                SellerId = sellerId,  // ✅ أخدناها من الـ Claims
                CategoryId = dto.CategoryId,
                ImageId = imageId
            };

            var added = _repo.ADD(entity);
            _repo.SavaChange();
            return ToDto(added);
        }

        // ✅ تعديل خدمة
        public ServiceDto Update(int id, UpdateServiceDto dto)
        {
            var existing = _repo.Getbyid(id);
            if (existing == null) return null;

            // 🛑 نتحقق إن اللي بيعدل هو نفس صاحب الخدمة
            var sellerId = GetCurrentUserId();
            if (existing.SellerId != sellerId)
                throw new UnauthorizedAccessException("You cannot edit someone else's service!");

            // ✅ لو الصورة اتغيرت ارفع الجديدة
            if (dto.File != null)
            {
                var img = new Image
                {
                    FileName = Path.GetFileNameWithoutExtension(dto.File.FileName),
                    FileExtension = Path.GetExtension(dto.File.FileName),
                    FileSize = dto.File.Length,
                    File = dto.File
                };

                var savedImage = _imageRepo.Upload(img).Result;
                existing.ImageId = savedImage.Id;
            }

            // نحدّث باقي البيانات
            existing.Name = dto.Title;
            existing.Description = dto.Description;
            existing.BasePrice = dto.BasePrice;
            existing.DeliveryTime = dto.DeliveryTime;
            existing.Status = dto.Status ?? existing.Status;
            existing.CategoryId = dto.CategoryId;

            var updated = _repo.UPDATE(existing);
            _repo.SavaChange();
            return ToDto(updated);
        }

        // ✅ جلب كل الخدمات
        public IEnumerable<ServiceDto> GetAll()
        {
            var services = _repo.GetAll();
            return services.Select(ToDto);
        }

        // ✅ جلب خدمة واحدة بالـ ID
        public ServiceDto GetByID(int id)
        {
            var service = _repo.Getbyid(id);
            return service == null ? null : ToDto(service);
        }

        // ✅ حذف خدمة
        public bool Delete(int id)
        {
            var existing = _repo.Getbyid(id);
            if (existing == null) return false;

            var sellerId = GetCurrentUserId();
            if (existing.SellerId != sellerId)
                throw new UnauthorizedAccessException("You cannot delete someone else's service!");

            var deleted = _repo.Delete(id);
            if (!deleted) return false;

            _repo.SavaChange();
            return true;
        }

        // ✅ جلب كل خدمات Seller معيّن
        public IEnumerable<ServiceDto> GetAllBySellerId(string sellerId)
        {
            var services = _repo.GetAllBySellerId(sellerId);
            return services.Select(ToDto);
        }

        // ✅ جلب كل خدمات Category معيّن
        public IEnumerable<ServiceDto> GetAllByCategoryId(int categoryId)
        {
            var services = _repo.GetAllByCategoryId(categoryId);
            return services.Select(ToDto);
        }

        public IEnumerable<ServiceDto> GetMyServices()
        {
            var sellerId = GetCurrentUserId();
            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException("User is not authenticated!");

            var services = _repo.GetAllBySellerId(sellerId);
            return services.Select(ToDto);
        }

    }
}
