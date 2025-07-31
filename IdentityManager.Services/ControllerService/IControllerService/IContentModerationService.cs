using Models.Domain;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface IContentModerationService
    {
        /// <summary>
        /// Simple content moderation that returns "Safe" or "Unsafe"
        /// </summary>
        /// <param name="input">Text content to moderate</param>
        /// <returns>"Safe" if content is appropriate, "Unsafe" if flagged</returns>
        Task<string> ModerateContentAsync(string input);

        /// <summary>
        /// Detailed content moderation with category-specific results
        /// </summary>
        /// <param name="input">Text content to moderate</param>
        /// <returns>Detailed moderation results</returns>
        Task<ContentModerationResult> ModerateContentDetailedAsync(string input);

        /// <summary>
        /// Moderate a product's title and description
        /// </summary>
        /// <param name="title">Product title</param>
        /// <param name="description">Product description</param>
        /// <returns>Product moderation results</returns>
        Task<ProductModerationResult> ModerateProductAsync(string title, string description);

        /// <summary>
        /// Batch moderation for multiple inputs
        /// </summary>
        /// <param name="inputs">Array of text content to moderate</param>
        /// <returns>Array of moderation results</returns>
        Task<ContentModerationResult[]> ModerateContentBatchAsync(string[] inputs);

        /// <summary>
        /// Check if content is safe (convenience method)
        /// </summary>
        /// <param name="title">Product title</param>
        /// <param name="description">Product description</param>
        /// <returns>True if content is safe, false otherwise</returns>
        Task<bool> IsContentSafeAsync(string title, string description);
    }
} 