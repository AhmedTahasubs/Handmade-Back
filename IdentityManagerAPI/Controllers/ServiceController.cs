using IdentityManager.Services.ControllerService;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Const;
using Models.DTOs;
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
        public IActionResult Create([FromForm] CreateServiceDto dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

       
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] UpdateServiceDto dto)
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
         
            var myServices = _service.GetMyServices();
            return Ok(myServices);
        }


        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public IActionResult GetAllByCategoryId(int categoryId)
        {
            var services = _service.GetAllByCategoryId(categoryId);
            return Ok(services);
        }
        [HttpGet("category/{categoryName:alpha}")]
        [AllowAnonymous]
        public IActionResult GetAllByCategoryName(string categoryName)
        {
            var services = _service.GetAllByCategoryName(categoryName);

            return Ok(services);
        }
        [HttpPatch("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateServiceStatus([FromRoute] int id, [FromForm] UpdateServiceStatusDTO dto)
        {
            try
            {
                var prod = await _service.UpdateServiceStatusAsync(id, dto);
                if (prod == null)
                    return NotFound();

                return Ok(new { message = "Status updated", status = prod.Status });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
