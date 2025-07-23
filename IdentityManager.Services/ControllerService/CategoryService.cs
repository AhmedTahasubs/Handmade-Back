using AutoMapper;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.Domain;
using Models.DTOs.Categories;

namespace IdentityManager.Services.ControllerService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

		public CategoryService(ICategoryRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
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

        public async Task<CategoryDto> CreateAsync(string? userId,CreateCategoryDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            entity.CreatedById = userId;
            var saved = await _repo.AddAsync(entity);
            return ToDto(saved);
        }

        public async Task<CategoryDto?> UpdateAsync(string? userId, int id, CategoryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.LastUpdatedOn = DateTime.Now;
            existing.LastUpdatedById = userId;
            var updated = await _repo.UpdateAsync(existing);
            return updated == null ? null : ToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }
}
