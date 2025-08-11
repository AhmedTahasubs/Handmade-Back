using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using DataAcess;
using DataAcess.Repos.IRepos;
using Aspose.Slides.Export;

namespace IdentityManagerAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymobService _paymobService;
		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _context;
		private readonly ILogger<PaymentController> _logger;
		private readonly ICustomerOrderService _orderService;

		public PaymentController(
			IPaymobService paymobService,
			IConfiguration configuration,
			ApplicationDbContext context,
			ILogger<PaymentController> logger,
			ICustomerOrderService orderService)
		{
			_paymobService = paymobService;
			_configuration = configuration;
			_context = context;
			_logger = logger;
			_orderService = orderService;
		}


		[Authorize]
		[HttpPost("create-payment-token")]
		public async Task<IActionResult> CreatePaymentToken(
			[FromQuery] int orderID,
			[FromQuery] string paymentMethod)
		{
			if (orderID <= 0)
				return BadRequest("Invalid Order ID.");

			var customertId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(customertId))
				return Unauthorized("User not authenticated.");

			var order = await _orderService.GetOrderByIdAsync(orderID);
			if (order == null)
				return NotFound("Enrollment not found.");

			try
			{
				var amount = order.TotalPrice;
				decimal totalAmount = amount; // Initialize totalAmount with the default value of amount.

				

				if (string.IsNullOrWhiteSpace(paymentMethod))
					return BadRequest("Payment method is required.");

				if (paymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase) ||
					paymentMethod.Equals("wallet", StringComparison.OrdinalIgnoreCase))
				{
					var (orderResult, redirectUrl) = await _paymobService.ProcessPaymentAsync(order.Id, paymentMethod);
					return Ok(new { RedirectUrl = redirectUrl });
				}
				else
				{
					return BadRequest("Invalid payment method. Supported methods are 'card' and 'wallet'.");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"Error processing payment: {ex.Message}");
			}
		}

		/// <summary>
		/// Handles the user redirect callback after payment.
		/// Displays success or failure page.
		/// </summary>
		[HttpGet("callback")]
		public async Task<IActionResult> CallbackAsync()
		{
			var query = Request.Query;

			string[] fields = new[]
			{
				"amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
				"id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
				"is_standalone_payment", "is_voided", "order", "owner", "pending",
				"source_data.pan", "source_data.sub_type", "source_data.type", "success"
			};

			var concatenated = new StringBuilder();
			foreach (var field in fields)
			{
				if (query.TryGetValue(field, out var value))
				{
					concatenated.Append(value);
				}
				else
				{
					return BadRequest($"Missing expected field: {field}");
				}
			}

			string receivedHmac = query["hmac"];
			string calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated.ToString(), _configuration["Paymob:HMAC"]);

			if (receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
			{
				bool.TryParse(query["success"], out bool isSuccess);
				var specialReference = query["merchant_order_id"];

				if (isSuccess)
				{
					return Content(HtmlGenerator.GenerateSuccessHtml(), "text/html");
				}

				return Content(HtmlGenerator.GenerateFailedHtml(), "text/html");
			}

			return Content(HtmlGenerator.GenerateSecurityHtml(), "text/html");
		}

		/// <summary>
		/// Handles Paymob server-to-server callback (processed transaction).
		/// This ensures DB updates even if user never returns.
		/// </summary>
		[HttpPost("server-callback")]
		public async Task<IActionResult> ServerCallback([FromBody] JsonElement payload)
		{
			try
			{
				string receivedHmac = Request.Query["hmac"];
				string secret = _configuration["Paymob:HMAC"];

				if (!payload.TryGetProperty("obj", out var obj))
					return BadRequest("Missing 'obj' in payload.");

				string[] fields = new[]
				{
					"amount_cents", "created_at", "currency", "error_occured", "has_parent_transaction",
					"id", "integration_id", "is_3d_secure", "is_auth", "is_capture", "is_refunded",
					"is_standalone_payment", "is_voided", "order.id", "owner", "pending",
					"source_data.pan", "source_data.sub_type", "source_data.type", "success"
				};

				var concatenated = new StringBuilder();
				foreach (var field in fields)
				{
					string[] parts = field.Split('.');
					JsonElement current = obj;
					bool found = true;
					foreach (var part in parts)
					{
						if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(part, out var next))
							current = next;
						else
						{
							found = false;
							break;
						}
					}

					if (!found || current.ValueKind == JsonValueKind.Null)
					{
						concatenated.Append(""); // Use empty string for missing/null fields
					}
					else if (current.ValueKind == JsonValueKind.True || current.ValueKind == JsonValueKind.False)
					{
						concatenated.Append(current.GetBoolean() ? "true" : "false"); // Lowercase boolean
					}
					else
					{
						concatenated.Append(current.ToString());
					}
				}

				string calculatedHmac = _paymobService.ComputeHmacSHA512(concatenated.ToString(), secret);

				if (!receivedHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase))
					return Unauthorized("Invalid HMAC");

				string merchantOrderId = null;
				if (obj.TryGetProperty("order", out var order) &&
					order.TryGetProperty("merchant_order_id", out var merchantOrderIdElement) &&
					merchantOrderIdElement.ValueKind != JsonValueKind.Null)
				{
					merchantOrderId = merchantOrderIdElement.ToString();
				}

				bool isSuccess = obj.TryGetProperty("success", out var successElement) && successElement.GetBoolean();

				if (!string.IsNullOrEmpty(merchantOrderId))
				{
					if (isSuccess)
						await _paymobService.UpdateOrderSuccess(merchantOrderId);
					else
						await _paymobService.UpdateOrderFailed(merchantOrderId);
				}

				return Ok();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error processing server callback: {ex.Message}");
			}
		}
	}
}
