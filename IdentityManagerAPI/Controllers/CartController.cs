using DataAcess.Repos.IRepos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Domain;
using Models.DTOs.Cart;
using Models.DTOs.CartItem;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(int id)
        {
            var cart = await _cartRepository.GetByIdAsync(id);
            if (cart == null)
                return NotFound();

            return Ok(new CartDto
            {
                Id = cart.Id,
                BuyerId = cart.BuyerId,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items.Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            });
        }

        [HttpGet("buyer/{buyerId}")]
        public async Task<IActionResult> GetCartByBuyer(string buyerId)
        {
            var cart = await _cartRepository.GetByBuyerIdAsync(buyerId);
            if (cart == null)
                return NotFound();

            return Ok(new CartDto
            {
                Id = cart.Id,
                BuyerId = cart.BuyerId,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items.Select(item => new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartCreateDto cartDto)
        {
            var newCart = new Cart
            {
                BuyerId = cartDto.BuyerId,
                CreatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(newCart);
            return Ok(newCart);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _cartRepository.GetAsync(c => c.Id == id);
            if (cart == null)
                return NotFound();

            await _cartRepository.DeleteAsync(cart);
            return NoContent();
        }
    }
}
