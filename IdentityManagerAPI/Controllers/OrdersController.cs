using AutoMapper;


using Microsoft.AspNetCore.Mvc;
using DataAcess.Repos.IRepos;
using Models.DTOs.OrderDTO;
using Models.Domain;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public OrdersController(IOrderRepository orderRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll()
        {
            var orders = await _orderRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<OrderReadDto>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(_mapper.Map<OrderReadDto>(order));
        }

        [HttpPost]
        public async Task<ActionResult> Create(OrderCreateDto dto)
        {
            var order = _mapper.Map<Order>(dto);
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, OrderCreateDto dto)
        {
            var existing = await _orderRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            _orderRepo.Update(existing);
            await _orderRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();
            _orderRepo.DeleteAsync(order);
            await _orderRepo.SaveChangesAsync();
            return NoContent();
        }
    }
}
