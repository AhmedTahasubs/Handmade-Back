using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.Domain;
using Models.DTOs.Category;

namespace IdentityManager.Services.ControllerService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        // ✅ تحويل Entity → DTO
        private static CategoryDto ToDto(Category c) => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        };

        // ✅ تحويل DTO → Entity
        private static Category ToEntity(CategoryDto dto) => new Category
        {
            Id = dto.Id,
            Name = dto.Name
        };

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(c => ToDto(c));
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var cat = await _repo.GetByIdAsync(id);
            return cat == null ? null : ToDto(cat);
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto dto)
        {
            var entity = ToEntity(dto);
            var saved = await _repo.AddAsync(entity);
            return ToDto(saved);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, CategoryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            var updated = await _repo.UpdateAsync(existing);
            return updated == null ? null : ToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}
