using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Models.DTOs.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly CartService _cartService;

        public ShoppingCartController(CartService cartService)
        {
            _cartService = cartService;
        }

        private string GetCustomerId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync(GetCustomerId());
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(AddCartItemDto dto)
        {
            await _cartService.AddProductAsync(GetCustomerId(), dto.ProductId, dto.Quantity, dto.UnitPrice);
            return Ok();
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItem(int itemId, UpdateCartItemDto dto)
        {
            await _cartService.UpdateQuantityAsync(GetCustomerId(), itemId, dto.Quantity);
            return Ok();
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            await _cartService.RemoveItemAsync(GetCustomerId(), itemId);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearCartAsync(GetCustomerId());
            return Ok();
        }
    }
}
