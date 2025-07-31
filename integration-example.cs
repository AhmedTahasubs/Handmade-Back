using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityManager.Services.ControllerService
{
    public interface IContentModerationService
    {
        Task<bool> IsContentSafeAsync(string title, string description);
        Task<ModerationResult> ModerateContentAsync(string title, string description);
    }

    public class ModerationResult
    {
        public bool IsSafe { get; set; }
        public bool TitleSafe { get; set; }
        public bool DescriptionSafe { get; set; }
        public string[] FlaggedCategories { get; set; } = Array.Empty<string>();
        public string? Error { get; set; }
    }

    public class ContentModerationService : IContentModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContentModerationService> _logger;
        private readonly string _moderationServiceUrl;

        public ContentModerationService(
            HttpClient httpClient, 
            IConfiguration configuration,
            ILogger<ContentModerationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _moderationServiceUrl = configuration["ModerationService:Url"] ?? "http://localhost:3000";
        }

        public async Task<bool> IsContentSafeAsync(string title, string description)
        {
            try
            {
                var result = await ModerateContentAsync(title, description);
                return result.IsSafe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking content safety");
                return true; // Default to safe during errors
            }
        }

        public async Task<ModerationResult> ModerateContentAsync(string title, string description)
        {
            try
            {
                var request = new
                {
                    title = title ?? string.Empty,
                    description = description ?? string.Empty,
                    sellerId = "system" // You can pass actual seller ID if needed
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_moderationServiceUrl}/moderate-product", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ModerationResult>(responseContent);
                    
                    if (result != null)
                    {
                        _logger.LogInformation("Content moderation completed. Safe: {IsSafe}, TitleSafe: {TitleSafe}, DescriptionSafe: {DescriptionSafe}", 
                            result.IsSafe, result.TitleSafe, result.DescriptionSafe);
                        return result;
                    }
                }

                _logger.LogWarning("Moderation service returned non-success status: {StatusCode}", response.StatusCode);
                return new ModerationResult { IsSafe = true, TitleSafe = true, DescriptionSafe = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling moderation service");
                return new ModerationResult 
                { 
                    IsSafe = true, // Default to safe during errors
                    TitleSafe = true, 
                    DescriptionSafe = true,
                    Error = ex.Message
                };
            }
        }
    }
}

// Example usage in ProductController
namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IContentModerationService _moderationService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            IContentModerationService moderationService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _moderationService = moderationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductCreateDTO productCreateDTO)
        {
            try
            {
                // Moderate content before creating product
                var moderationResult = await _moderationService.ModerateContentAsync(
                    productCreateDTO.Title, 
                    productCreateDTO.Description
                );

                if (!moderationResult.IsSafe)
                {
                    var flaggedCategories = string.Join(", ", moderationResult.FlaggedCategories);
                    _logger.LogWarning("Product content flagged as inappropriate. Categories: {Categories}", flaggedCategories);
                    
                    return BadRequest(new 
                    { 
                        message = "Product content violates community guidelines",
                        flaggedCategories = moderationResult.FlaggedCategories,
                        details = new
                        {
                            titleSafe = moderationResult.TitleSafe,
                            descriptionSafe = moderationResult.DescriptionSafe
                        }
                    });
                }

                // Content is safe, proceed with product creation
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Unauthorized();

                var productDTO = await _productService.Create(productCreateDTO, userId);
                return CreatedAtAction(nameof(GetById), new { id = productDTO.Id }, productDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "An error occurred while creating the product");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromForm] ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                if (id != productUpdateDTO.Id)
                    return BadRequest("ID mismatch between route and payload.");

                var existingProduct = await _productService.GetById(id);
                if (existingProduct == null)
                    return NotFound();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole(AppRoles.Admin);

                if (userId == null)
                    return Unauthorized();

                if (!isAdmin && userId != existingProduct.SellerId)
                    return Forbid();

                // Moderate updated content
                var moderationResult = await _moderationService.ModerateContentAsync(
                    productUpdateDTO.Title, 
                    productUpdateDTO.Description
                );

                if (!moderationResult.IsSafe)
                {
                    var flaggedCategories = string.Join(", ", moderationResult.FlaggedCategories);
                    _logger.LogWarning("Updated product content flagged as inappropriate. Product ID: {ProductId}, Categories: {Categories}", 
                        id, flaggedCategories);
                    
                    return BadRequest(new 
                    { 
                        message = "Updated product content violates community guidelines",
                        flaggedCategories = moderationResult.FlaggedCategories
                    });
                }

                // Content is safe, proceed with update
                var productDisplayDTO = await _productService.Update(productUpdateDTO);
                return Ok(productDisplayDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, "An error occurred while updating the product");
            }
        }
    }
}

// Program.cs registration
namespace IdentityManagerAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ... existing configuration ...

            // Add content moderation service
            builder.Services.AddHttpClient<IContentModerationService, ContentModerationService>();
            
            // ... rest of configuration ...
        }
    }
}

// appsettings.json configuration
/*
{
  "ModerationService": {
    "Url": "http://localhost:3000",
    "Timeout": 30,
    "RetryCount": 3
  }
}
*/ 