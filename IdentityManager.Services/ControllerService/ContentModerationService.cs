using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.Extensions.Logging;
using Models.Domain;

namespace IdentityManager.Services.ControllerService
{
    public class ContentModerationService : IContentModerationService
    {
        private readonly CohereModerator _cohereModerator;
        private readonly ILogger<ContentModerationService> _logger;

        public ContentModerationService(CohereModerator cohereModerator, ILogger<ContentModerationService> logger)
        {
            _cohereModerator = cohereModerator;
            _logger = logger;
        }

        public async Task<string> ModerateContentAsync(string input)
        {
            try
            {
                _logger.LogInformation("Moderating content: {InputLength} characters", input?.Length ?? 0);
                var result = await _cohereModerator.ModerateContentAsync(input ?? string.Empty);
                _logger.LogInformation("Content moderation result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in content moderation");
                return "Safe"; // Default to safe during errors
            }
        }

        public async Task<ContentModerationResult> ModerateContentDetailedAsync(string input)
        {
            try
            {
                _logger.LogInformation("Detailed moderation for content: {InputLength} characters", input?.Length ?? 0);
                var result = await _cohereModerator.ModerateContentDetailedAsync(input ?? string.Empty);
                
                if (result.Flagged)
                {
                    _logger.LogWarning("Content flagged as inappropriate. Categories: {Categories}", 
                        string.Join(", ", result.Categories.Where(c => c.Value).Select(c => c.Key)));
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in detailed content moderation");
                return new ContentModerationResult
                {
                    IsSafe = true, // Default to safe during errors
                    Flagged = false,
                    Categories = new Dictionary<string, bool>(),
                    CategoryScores = new Dictionary<string, float>(),
                    Error = ex.Message
                };
            }
        }

        public async Task<ProductModerationResult> ModerateProductAsync(string title, string description)
        {
            try
            {
                _logger.LogInformation("Moderating product - Title: {TitleLength} chars, Description: {DescriptionLength} chars", 
                    title?.Length ?? 0, description?.Length ?? 0);
                
                var result = await _cohereModerator.ModerateProductAsync(title ?? string.Empty, description ?? string.Empty);
                
                if (!result.IsSafe)
                {
                    _logger.LogWarning("Product content flagged as inappropriate. Flagged categories: {Categories}", 
                        string.Join(", ", result.FlaggedCategories));
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moderating product");
                return new ProductModerationResult
                {
                    IsSafe = false, // Default to unsafe during errors
                    TitleSafe = false,
                    DescriptionSafe = false,
                    FlaggedCategories = new List<string> { "error" },
                    Error = ex.Message
                };
            }
        }

        public async Task<ContentModerationResult[]> ModerateContentBatchAsync(string[] inputs)
        {
            try
            {
                _logger.LogInformation("Batch moderating {Count} inputs", inputs?.Length ?? 0);
                var results = await _cohereModerator.ModerateContentBatchAsync(inputs ?? Array.Empty<string>());
                
                var flaggedCount = results.Count(r => r.Flagged);
                if (flaggedCount > 0)
                {
                    _logger.LogWarning("Batch moderation found {FlaggedCount} flagged items out of {TotalCount}", 
                        flaggedCount, results.Length);
                }
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch content moderation");
                return Array.Empty<ContentModerationResult>();
            }
        }

        public async Task<bool> IsContentSafeAsync(string title, string description)
        {
            try
            {
                var result = await ModerateProductAsync(title, description);
                return result.IsSafe;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking content safety");
                return true; // Default to safe during errors
            }
        }
    }
} 