using System.Security.Claims;
using DataAcess.Repos.IRepos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Domain;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly ICustomerOrderService _orderService;

        public CustomerOrdersController(ICustomerOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized("Customer ID not found in claims.");

            request.CustomerId = customerId;

            var order = await _orderService.CreateOrderAsync(request);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetOrdersByCustomer()
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized("Customer ID not found in claims.");
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        [HttpGet("seller")]
        public async Task<IActionResult> GetOrdersBySeller()
        {
            var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(sellerId))
                return Unauthorized("Seller ID not found in claims.");
            var items = await _orderService.GetOrdersBySellerAsync(sellerId);
            return Ok(items);
        }
        [HttpGet("seller/{id}")]
        public async Task<IActionResult> GetOrdersBySeller(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound(new { message = "seller Not Found" });
            var items = await _orderService.GetOrdersBySellerAsync(id);
            return Ok(items);
        }

        [HttpPatch("items/{orderItemId}/status")]
        public async Task<IActionResult> UpdateOrderItemStatus(int orderItemId, [FromBody] UpdateOrderItemStatusRequest request)
        {
            var success = await _orderService.UpdateOrderItemStatusAsync(orderItemId, request.Status);
            if (!success)
                return NotFound(new { message = "Order item not found." });

            return Ok(new { message = "Status updated successfully." });
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Order not found." });

            return Ok(order);
        }
        [HttpGet("seller/items")]
        public async Task<IActionResult> GetitemsBySeller()
        {
            var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(sellerId))
                return Unauthorized("Seller ID not found in claims.");
            var items = await _orderService.GetItemsBySeller(sellerId);
            return Ok(items);
        }
    }
}
