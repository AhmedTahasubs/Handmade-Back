using IdentityManager.Services.ControllerService;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.ServiceReview;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceReviewController : ControllerBase
    {
        private readonly IServiceReviewService service;

        public ServiceReviewController(IServiceReviewService service)
        {
         this.service = service;   
        }
        [HttpGet]
        public IActionResult GetAll()=>Ok(service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetByid(int id)
        {
            var temp = service.GetById(id);
            if(temp == null) { return NotFound(); }
            return Ok(temp);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateServiceReviewDto dto)
        {
            var created = service.Create(dto);  
            return CreatedAtAction(nameof(GetByid), new {id = created.Id},created);

        }
        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromBody] UpdateServiceReviewDto dto) {
        
            var updated = service.Update(id, dto); 
            if(updated == null)  return NotFound(); 
            return Ok(updated);
        
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) { 
            var deleted = service.Delete(id);
            if (deleted) return Ok();
            return NotFound();
        }

        

        
    }
}
