using System.Security.Claims;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Categories;

namespace IdentityManagerAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return Ok(await _service.CreateAsync(userId,dto));
		}


        [HttpPut("{id}")]
        public async Task<IActionResult> Update( int id, [FromBody] CategoryDto dto)
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return Ok(await _service.UpdateAsync(userId,id, dto));
		}
            

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => Ok(await _service.DeleteAsync(id));
    }

}
