using Models.DTOs;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface ISearchService
    {
        Task<IEnumerable<ProductDisplayDTO>> SearchProductsAsync(string query, int maxResults = 10);
        Task UpdateProductEmbeddingsAsync(int productId);
        Task UpdateAllProductEmbeddingsAsync();
        Task<float> CalculateCosineSimilarity(float[] vector1, float[] vector2);
    }
} 