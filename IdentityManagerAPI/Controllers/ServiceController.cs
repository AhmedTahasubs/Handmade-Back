using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Service;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous] 
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetById(int id)
        {
            var serv = _service.GetByID(id);
            if (serv == null) return NotFound();
            return Ok(serv);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateServiceDto dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateServiceDto dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var deleted = _service.Delete(id);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }

        [HttpGet("seller")]
        public IActionResult GetMyServices()
        {
          
            var services = _service.GetAllBySellerId(User.Identity.Name ?? "");
            return Ok(services);
        }

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public IActionResult GetAllByCategoryId(int categoryId)
        {
            var services = _service.GetAllByCategoryId(categoryId);
            return Ok(services);
        }
    }
}
