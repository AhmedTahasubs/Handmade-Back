using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Service;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // كل الـ APIs محمية ما عدا اللي عليها AllowAnonymous
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService service)
        {
            _service = service;
        }

        // ✅ جلب كل الخدمات (بدون تسجيل دخول)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll() => Ok(_service.GetAll());

        // ✅ جلب خدمة واحدة بالـ ID
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

        // ✅ تعديل خدمة + ممكن تغير الصورة
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] UpdateServiceDto dto)
        {
            var updated = _service.Update(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // ✅ حذف خدمة
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            var deleted = _service.Delete(id);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }

        // ✅ جلب كل الخدمات الخاصة بالـ Seller الحالي
        [HttpGet("seller")]
        public IActionResult GetMyServices()
        {
            // ✅ مش هنجيب SellerId هنا، السيرفس نفسه بياخده من الـ Claims
            var myServices = _service.GetMyServices();
            return Ok(myServices);
        }

        // ✅ جلب خدمات كاتيجوري معيّن
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
    }
    }
