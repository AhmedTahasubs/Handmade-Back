using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Domain;
using Models.DTOs.Categories;
using Models.DTOs.Service;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService service;

        public ServiceController(IServiceService service)
        {
            this.service = service;
        }
        [HttpGet]
        public IActionResult GetAll() => Ok(service.GetAll());
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var serv = service.GetByID(id);
            if (serv == null)
                return NotFound();
            return Ok(serv);

        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateServiceDto dto) {

            var created = service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);

        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateServiceDto dto) { 
       
            var Update = service.Update(id, dto);
            if (Update == null) return NotFound();
            return Ok(Update);

            
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id) {
        
            var deleted = service.Delete(id);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }

      
    }
}
