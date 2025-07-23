
using AutoMapper;

using DataAcess.Repos.IRepos;
using Models.DTOs.CustomRequestDTO;
using Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomRequestsController : ControllerBase
    {
        private readonly ICustomRequestRepository _customRepo;
        private readonly IMapper _mapper;

        public CustomRequestsController(ICustomRequestRepository customRepo, IMapper mapper)
        {
            _customRepo = customRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomRequestReadDto>>> GetAll()
        {
            var requests = await _customRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CustomRequestReadDto>>(requests));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomRequestReadDto>> GetById(int id)
        {
            var request = await _customRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(_mapper.Map<CustomRequestReadDto>(request));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CustomRequestCreateDto dto)
        {
            var request = _mapper.Map<CustomRequest>(dto);
            await _customRepo.AddAsync(request);
            await _customRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CustomRequestCreateDto dto)
        {
            var existing = await _customRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _customRepo.Update(existing);
            await _customRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var request = await _customRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            _customRepo.DeleteAsync(request);
            await _customRepo.SaveChangesAsync();
            return NoContent();
        }
    }
}
