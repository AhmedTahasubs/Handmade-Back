using AutoMapper;


using Microsoft.AspNetCore.Mvc;
using DataAcess.Repos.IRepos;
using Models.DTOs.OrderItemDTO;
using Models.Domain;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemRepository _itemRepo;
        private readonly IMapper _mapper;

        public OrderItemsController(IOrderItemRepository itemRepo, IMapper mapper)
        {
            _itemRepo = itemRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemReadDto>>> GetAll()
        {
            var items = await _itemRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<OrderItemReadDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemReadDto>> GetById(int id)
        {
            var item = await _itemRepo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<OrderItemReadDto>(item));
        }

        [HttpPost]
        public async Task<ActionResult> Create(OrderItemCreateDto dto)
        {
            var item = _mapper.Map<OrderItem>(dto);
            await _itemRepo.AddAsync(item);
            await _itemRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, OrderItemCreateDto dto)
        {
            var existing = await _itemRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _itemRepo.Update(existing);
            await _itemRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _itemRepo.GetByIdAsync(id);
            if (item == null) return NotFound();
            _itemRepo.DeleteAsync(item);
            await _itemRepo.SaveChangesAsync();
            return NoContent();
        }
    }
}
