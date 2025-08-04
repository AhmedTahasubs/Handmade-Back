using System.Security.Claims;
using DataAcess.Repos.IRepos;
using DataAcess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Domain;
using Models.DTOs.CustomerReqestDTOs;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRequestsController : ControllerBase
    {
        private readonly ICustomerRequestService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerRequestsController(ICustomerRequestService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet("all")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<CustomerRequestResponse>>> GetAll()
        {
            var requests = await _service.GetAllAsync();
            return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCustomerRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.CreateAsync(dto, userId);
            return Ok(result);
        }

        [HttpGet("by-seller")]
        [Authorize]
        public async Task<IActionResult> GetBySeller()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetBySellerIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("by-customer")]
        [Authorize]
        public async Task<IActionResult> GetByCustomer()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _service.GetByCustomerIdAsync(userId);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var success = await _service.UpdateStatusAsync(id, status);
            return success ? Ok() : NotFound();
        }
    }
}
