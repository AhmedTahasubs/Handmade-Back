using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models.DTOs.Category;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryDto dto);
        Task<CategoryDto> UpdateAsync(int id, CategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
