using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataAcess;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Domain;
using X.Paymob.CashIn;



namespace IdentityManager.Services.ControllerService
{
	public class PaymobService : IPaymobService
	{
		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _context;
		private readonly IMailingService _emailService;
		private readonly UserManager<ApplicationUser> _userManager;


		public PaymobService(
			ApplicationDbContext context,
			IConfiguration configuration,
			IPaymobCashInBroker broker,
			IMailingService emailService,
			UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_configuration = configuration;
			_emailService = emailService;
			_userManager = userManager;
		}

		public async Task<(Payment payment, string RedirectUrl)> ProcessPaymentAsync(int orderId, string paymentMethod)
		{
			var order = await _context.CustomerOrders
				.Include(e => e.Items)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(e => e.Id == orderId);

			if (order == null)
				throw new KeyNotFoundException($"Order with ID {orderId} not found.");

			var customerId = order.CustomerId ?? throw new InvalidOperationException("Order customer not found.");

			var customer = _userManager.FindByIdAsync(customerId).Result;
			// Create HTTP client for direct API calls to Paymob
			var httpClient = new HttpClient();

			// Get API key from configuration
			string apiKey = _configuration["Paymob:APIKey"] ??
				throw new ArgumentException("Paymob API key not configured");

			string secretKey = _configuration["Paymob:SecretKey"] ??
				throw new ArgumentException("Paymob secret key not configured");

			string publicKey = _configuration["Paymob:PublicKey"] ??
				throw new ArgumentException("Paymob public key not configured");

			// Generate a special reference for this transaction
			int specialReference = RandomNumberGenerator.GetInt32(1000000, 9999999) + orderId;


			var amountCents = (int)(order.TotalAmount * 100);

			// Prepare billing data
			var billingData = new
			{
				apartment = "N/A",
				first_name = customer.FullName ?? "Guest",
				last_name = customer.FullName ?? "User",
				street = "N/A",
				building = "N/A",
				phone_number = customer.PhoneNumber,
				country = customer.Address,
				email = customer.Email,
				floor = "N/A",
				state = "N/A",
				city = "N/A"
			};

			// Get wallet integration ID
			var integrationId = int.Parse(DetermineIntegrationId(paymentMethod));

			var items = order.Items.Select(oi => new
			{
				name = oi.Product.Title,
				amount = (int)(oi.UnitPrice * 100),
				description = oi.Product.Description ?? "Product",
				quantity = oi.Quantity
			});

			// Prepare intention request payload
			var payload = new
			{
				amount = amountCents,
				currency = "EGP",
				payment_methods = new[] { integrationId },
				billing_data = billingData,
				items = items,
				customer = new
				{
					first_name = billingData.first_name,
					last_name = billingData.last_name,
					email = billingData.email,
					extras = new { orderId = order.Id }
				},
				extras = new
				{
					orderId = order.Id,
					customerId = customer.Id
				},
				special_reference = specialReference,
				expiration = 3600, // 1 hour expiration
				merchant_order_id = specialReference.ToString()
			};

			// Create HTTP request for Paymob's intention API
			var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Token", secretKey);
			requestMessage.Content = JsonContent.Create(payload);

			// Send the request and process response
			var response = await httpClient.SendAsync(requestMessage);
			var responseContent = await response.Content.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Paymob Intention API call failed with status {response.StatusCode}: {responseContent}");
			}

			// Parse the response to get client_secret
			var resultJson = JsonDocument.Parse(responseContent);
			var clientSecret = resultJson.RootElement.GetProperty("client_secret").GetString();

			// Create payment record
			var payment = new Payment
			{
				Amount = order.TotalAmount,
				PaymentMethod = paymentMethod,
				Status = "Pending",
				TransactionId = specialReference.ToString(),
				OrderId = order.Id,
				PaymentDate = DateTime.Now,
				UserId = customer.Id
			};

			_context.Payments.Add(payment);
			order.PaymentStatus = "Pending";
			await _context.SaveChangesAsync();

			// Generate payment URL for the unified checkout
			string redirectUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

			return (payment, redirectUrl);
		}

		private string DetermineIntegrationId(string paymentMethod)
		{
			return paymentMethod?.ToLower() switch
			{
				"card" => _configuration["Paymob:CardIntegrationId"] ?? throw new ArgumentException("Card integration ID not configured"),
				"wallet" => _configuration["Paymob:MobileIntegrationId"] ?? throw new ArgumentException("Wallet integration ID not configured"),
				_ => throw new ArgumentException($"Invalid payment method: {paymentMethod}")
			};
		}

		public string ComputeHmacSHA512(string data, string secret)
		{
			var keyBytes = Encoding.UTF8.GetBytes(secret);
			var dataBytes = Encoding.UTF8.GetBytes(data);

			using (var hmac = new HMACSHA512(keyBytes))
			{
				var hash = hmac.ComputeHash(dataBytes);
				return BitConverter.ToString(hash).Replace("-", "").ToLower();
			}
		}

		public async Task<CustomerOrder> UpdateOrderSuccess(string specialReference)
		{
			var payment = await _context.Payments
				.Include(p => p.Order)
				.ThenInclude(o => o.Items)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(p => p.TransactionId == specialReference);

			if (payment == null)
			{
				throw new KeyNotFoundException($"Payment with transaction ID {specialReference} not found.");
			}

			var order = payment.Order;

			if (order == null)
			{
				throw new KeyNotFoundException($"Enrollment with ID {payment.OrderId} not found.");
			}

			// Update enrollment status and payment status
			order.PaymentStatus = "Success";
			payment.Status = "Success";

			var notfication = new
			{
				Title = "Order Confirmed 🎉",
				Message = $"Your order #{order.Id} has been confirmed successfully. Thank you for shopping with us!",
				CreatedById = "System",
				CreatedByName = "Handmade",
				CreatorImage = "https://i.ibb.co/YFr894B3/admin.png",
				TargetUserId = order.CustomerId,
				CreatedAt = DateTime.Now
			};

			await _context.SaveChangesAsync();

			// Send confirmation email
			await SendPaymentConfirmationEmail(payment);

			return order;
		}

		public async Task<CustomerOrder> UpdateOrderFailed(string specialReference)
		{
			var payment = await _context.Payments
				.Include(p => p.Order)
				.FirstOrDefaultAsync(p => p.TransactionId == specialReference);

			if (payment == null)
			{
				throw new KeyNotFoundException($"Payment with transaction ID {specialReference} not found.");
			}

			var order = await _context.CustomerOrders
				.FirstOrDefaultAsync(e => e.Id == payment.OrderId);

			if (order == null)
			{
				throw new KeyNotFoundException($"Enrollment with ID {payment.OrderId} not found.");
			}

			// Update enrollment status and payment status
			order.PaymentStatus = "Failed";
			payment.Status = "Failed";

			await _context.SaveChangesAsync();

			return order;
		}

		private async Task SendPaymentConfirmationEmail(Payment payment)
		{
			var customer = await _userManager.FindByIdAsync(payment.UserId);
			var order = await _context.CustomerOrders
					.Include(o => o.Items)
					.ThenInclude(oi => oi.Product)
					.FirstOrDefaultAsync(o => o.Id == payment.OrderId);

			if (customer != null && order != null)
			{
				var emailContent = $@"
                        <html>
                    <head>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                color: #333333;
                            }}
                            .container {{
                                padding: 20px;
                            }}
                            .footer {{
                                margin-top: 30px;
                                font-size: 14px;
                                color: #777777;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <p>Dear {customer.FullName},</p>
                    
                            <p>Thank you for your order!</p>
							<p>Your payment of <strong>{order.TotalAmount} EGP</strong> has been successfully processed for the following items:</p>	
							<p>Your order number is <strong>#{order.Id}</strong>. You will receive another email once your order is shipped.</p>
                            <div class='footer'>
                                <p>Best regards,<br>The Handmade Khamsat Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";

				await _emailService.SendEmailAsync(customer.Email!, "Payment Confirmation", emailContent);
			}
		}


	}
}
