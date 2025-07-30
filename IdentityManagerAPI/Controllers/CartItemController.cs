using DataAcess.Repos.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Domain;
using Models.DTOs.CartItem;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemController(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetItemsByCartId(int cartId)
        {
            var items = await _cartItemRepository.GetItemsByCartIdAsync(cartId);

            var result = items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });

            return Ok(result);
        }

        [HttpGet("cart/{cartId}/product/{productId}")]
        public async Task<IActionResult> GetItemByProduct(int cartId, int productId)
        {
            var item = await _cartItemRepository.GetItemByProductAsync(cartId, productId);
            if (item == null)
                return NotFound();

            return Ok(new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] CartItemCreateDto dto)
        {
            var newItem = new CartItem
            {
                CartId = dto.CartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            await _cartItemRepository.AddAsync(newItem);
            return Ok(newItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _cartItemRepository.GetAsync(i => i.Id == id);
            if (item == null)
                return NotFound();

            await _cartItemRepository.DeleteAsync(item);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] CartItemUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var existingItem = await _cartItemRepository.GetAsync(i => i.Id == id);
            if (existingItem == null)
                return NotFound();

            existingItem.Quantity = dto.Quantity;
            existingItem.UnitPrice = dto.UnitPrice;

            await _cartItemRepository.UpdateAsync(existingItem);

            return NoContent();
        }
    }
}
