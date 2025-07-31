using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Moderation;
using Models.Domain;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentModerationController : ControllerBase
    {
        private readonly IContentModerationService _moderationService;
        private readonly ILogger<ContentModerationController> _logger;

        public ContentModerationController(
            IContentModerationService moderationService,
            ILogger<ContentModerationController> logger)
        {
            _moderationService = moderationService;
            _logger = logger;
        }

        /// <summary>
        /// Debug endpoint to test moderation with known test cases
        /// </summary>
        /// <returns>Test results</returns>
        [HttpGet("debug-test")]
        public async Task<ActionResult<object>> DebugTest()
        {
            try
            {
                var testCases = new[]
                {
                    "Beautiful handmade wooden table for sale",
                    "This product contains explicit adult content and violence",
                    "Handcrafted jewelry made with love",
                    "Explicit sexual content here",
                    "Hate speech and violence in this description",
                    "Professional photography services for weddings"
                };

                var results = new List<object>();

                foreach (var testCase in testCases)
                {
                    var result = await _moderationService.ModerateContentDetailedAsync(testCase);
                    results.Add(new
                    {
                        input = testCase,
                        isSafe = result.IsSafe,
                        flagged = result.Flagged,
                        categories = result.Categories,
                        categoryScores = result.CategoryScores,
                        error = result.Error
                    });
                }

                return Ok(new
                {
                    message = "Debug test completed",
                    results = results,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug test");
                return StatusCode(500, "An error occurred during debug test");
            }
        }

        /// <summary>
        /// Test API key connectivity
        /// </summary>
        /// <returns>API key test result</returns>
        [HttpGet("test-api-key")]
        public async Task<ActionResult<object>> TestApiKey()
        {
            try
            {
                // Test with a simple, clearly unsafe text
                var testText = "This contains explicit sexual content and violence";
                var result = await _moderationService.ModerateContentDetailedAsync(testText);
                
                return Ok(new
                {
                    message = "API key test completed",
                    testText = testText,
                    result = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing API key");
                return StatusCode(500, new
                {
                    message = "API key test failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Test with specific entries
        /// </summary>
        /// <returns>Test results with various content types</returns>
        [HttpGet("test-entries")]
        public async Task<ActionResult<object>> TestEntries()
        {
            try
            {
                var testEntries = new[]
                {
                    // Safe e-commerce content
                    "Beautiful handmade wooden coffee table for sale",
                    "Professional photography services for weddings",
                    "Handcrafted leather wallet made with love",
                    "Artisan soap and bath products",
                    "Vintage clothing and accessories",
                    "Custom jewelry design services",
                    "Home decor and furniture collection",
                    "Professional consulting services",
                    
                    // Borderline content (should be safe)
                    "Adult education courses",
                    "Professional adult content writing services",
                    "Violence prevention workshops",
                    "Hate speech awareness training",
                    
                    // Unsafe content
                    "Explicit sexual content and adult material",
                    "Graphic violence and gore for sale",
                    "Hate speech and discrimination products",
                    "Illegal drugs and substances",
                    "Explicit nudity and pornography",
                    "Violence and attack weapons",
                    "Racist and discriminatory content"
                };

                var results = new List<object>();

                foreach (var entry in testEntries)
                {
                    var result = await _moderationService.ModerateContentDetailedAsync(entry);
                    results.Add(new
                    {
                        input = entry,
                        isSafe = result.IsSafe,
                        flagged = result.Flagged,
                        categories = result.Categories,
                        categoryScores = result.CategoryScores,
                        error = result.Error
                    });
                }

                var safeCount = results.Count(r => (bool)r.GetType().GetProperty("isSafe").GetValue(r));
                var unsafeCount = results.Count - safeCount;

                return Ok(new
                {
                    message = "Test entries completed",
                    totalTests = testEntries.Length,
                    safeCount = safeCount,
                    unsafeCount = unsafeCount,
                    results = results,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing entries");
                return StatusCode(500, new
                {
                    message = "Test entries failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Simple content moderation
        /// </summary>
        /// <param name="request">Content moderation request</param>
        /// <returns>Moderation result</returns>
        [HttpPost("moderate")]
        public async Task<ActionResult<ContentModerationResponseDto>> ModerateContent([FromBody] ContentModerationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _moderationService.ModerateContentAsync(request.Text);
                
                return Ok(new ContentModerationResponseDto
                {
                    Text = request.Text,
                    Result = result,
                    IsSafe = result == "Safe",
                    Flagged = result == "Unsafe",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating content");
                return StatusCode(500, "An error occurred while moderating content");
            }
        }

        /// <summary>
        /// Detailed content moderation with categories
        /// </summary>
        /// <param name="request">Content moderation request</param>
        /// <returns>Detailed moderation result</returns>
        [HttpPost("moderate-detailed")]
        public async Task<ActionResult<ContentModerationResponseDto>> ModerateContentDetailed([FromBody] ContentModerationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _moderationService.ModerateContentDetailedAsync(request.Text);
                
                return Ok(new ContentModerationResponseDto
                {
                    Text = request.Text,
                    Result = result.IsSafe ? "Safe" : "Unsafe",
                    IsSafe = result.IsSafe,
                    Flagged = result.Flagged,
                    Categories = result.Categories,
                    CategoryScores = result.CategoryScores,
                    Error = result.Error,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in detailed content moderation");
                return StatusCode(500, "An error occurred while moderating content");
            }
        }

        /// <summary>
        /// Moderate a product's title and description
        /// </summary>
        /// <param name="request">Product moderation request</param>
        /// <returns>Product moderation result</returns>
        [HttpPost("moderate-product")]
        public async Task<ActionResult<ProductModerationResponseDto>> ModerateProduct([FromBody] ProductModerationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _moderationService.ModerateProductAsync(request.Title, request.Description);
                
                return Ok(new ProductModerationResponseDto
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsSafe = result.IsSafe,
                    TitleSafe = result.TitleSafe,
                    DescriptionSafe = result.DescriptionSafe,
                    FlaggedCategories = result.FlaggedCategories,
                    Error = result.Error,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating product");
                return StatusCode(500, "An error occurred while moderating product");
            }
        }

        /// <summary>
        /// Batch moderation for multiple texts
        /// </summary>
        /// <param name="request">Batch moderation request</param>
        /// <returns>Batch moderation results</returns>
        [HttpPost("moderate-batch")]
        [Authorize(Roles = "Admin")] // Only admins can use batch moderation
        public async Task<ActionResult<BatchModerationResponseDto>> ModerateBatch([FromBody] BatchModerationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var results = await _moderationService.ModerateContentBatchAsync(request.Texts);
                
                var responseResults = results.Select((result, index) => new ContentModerationResponseDto
                {
                    Text = request.Texts[index],
                    Result = result.IsSafe ? "Safe" : "Unsafe",
                    IsSafe = result.IsSafe,
                    Flagged = result.Flagged,
                    Categories = result.Categories,
                    CategoryScores = result.CategoryScores,
                    Error = result.Error,
                    Timestamp = DateTime.UtcNow
                }).ToList();

                return Ok(new BatchModerationResponseDto
                {
                    Results = responseResults,
                    Count = responseResults.Count,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch moderation");
                return StatusCode(500, "An error occurred while processing batch moderation");
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Service status</returns>
        [HttpGet("health")]
        public ActionResult<object> Health()
        {
            return Ok(new
            {
                status = "OK",
                service = "Content Moderation",
                timestamp = DateTime.UtcNow
            });
        }
    }
} 