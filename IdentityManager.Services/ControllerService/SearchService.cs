using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.Domain;
using Models.DTOs;
using AutoMapper;
using System.Numerics;

namespace IdentityManager.Services.ControllerService
{
    public class SearchService : ISearchService
    {
        private readonly IProductRepository _productRepository;
        private readonly CohereEmbedder _cohereEmbedder;
        private readonly IMapper _mapper;

        public SearchService(
            IProductRepository productRepository,
            CohereEmbedder cohereEmbedder,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _cohereEmbedder = cohereEmbedder;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> SearchProductsAsync(string query, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<ProductDisplayDTO>();

            // Get query embedding
            var queryEmbedding = await _cohereEmbedder.GetQueryEmbeddingAsync(query);

            // Get all products with embeddings
            var allProducts = await _productRepository.GetAllProducts();
            var productsWithEmbeddings = allProducts.Where(p => 
                !string.IsNullOrEmpty(p.TitleEmbedding) && 
                !string.IsNullOrEmpty(p.DescriptionEmbedding) &&
                p.Status.ToLower() == "approved").ToList();

            if (!productsWithEmbeddings.Any())
                return Enumerable.Empty<ProductDisplayDTO>();

            // Calculate similarities and sort
            var productsWithSimilarity = new List<(Product Product, float Similarity)>();

            foreach (var product in productsWithEmbeddings)
            {
                var titleEmbedding = product.GetTitleEmbeddingArray();
                var descriptionEmbedding = product.GetDescriptionEmbeddingArray();

                if (titleEmbedding == null || descriptionEmbedding == null)
                    continue;

                // Calculate similarity with title (weighted more heavily)
                var titleSimilarity = await CalculateCosineSimilarity(queryEmbedding, titleEmbedding);
                
                // Calculate similarity with description
                var descriptionSimilarity = await CalculateCosineSimilarity(queryEmbedding, descriptionEmbedding);
                
                // Weighted average (title is more important)
                var weightedSimilarity = (titleSimilarity * 0.7f) + (descriptionSimilarity * 0.3f);
                
                productsWithSimilarity.Add((product, weightedSimilarity));
            }

            // Sort by similarity (descending) and take top results
            var topProducts = productsWithSimilarity
                .OrderByDescending(x => x.Similarity)
                .Take(maxResults)
                .Select(x => x.Product)
                .ToList();

            return _mapper.Map<IEnumerable<ProductDisplayDTO>>(topProducts);
        }

        public async Task UpdateProductEmbeddingsAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                return;

            await UpdateProductEmbeddingsInternalAsync(product);
            await _productRepository.UpdateProductAsync(product);
            await _productRepository.SaveAsync();
        }

        public async Task UpdateAllProductEmbeddingsAsync()
        {
            var allProducts = await _productRepository.GetAllProducts();
            
            foreach (var product in allProducts)
            {
                await UpdateProductEmbeddingsInternalAsync(product);
            }
            
            await _productRepository.SaveAsync();
        }

        private async Task UpdateProductEmbeddingsInternalAsync(Product product)
        {
            try
            {
                // Generate embeddings for title and description
                var titleEmbedding = await _cohereEmbedder.GetDocumentEmbeddingAsync(product.Title);
                var descriptionEmbedding = await _cohereEmbedder.GetDocumentEmbeddingAsync(product.Description);

                // Store embeddings
                product.SetTitleEmbeddingArray(titleEmbedding);
                product.SetDescriptionEmbeddingArray(descriptionEmbedding);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the entire operation
                // In production, you'd want proper logging here
                Console.WriteLine($"Failed to update embeddings for product {product.Id}: {ex.Message}");
            }
        }

        public async Task<float> CalculateCosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1.Length != vector2.Length)
                throw new ArgumentException("Vectors must have the same length");

            var dotProduct = 0f;
            var magnitude1 = 0f;
            var magnitude2 = 0f;

            for (int i = 0; i < vector1.Length; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = (float)Math.Sqrt(magnitude1);
            magnitude2 = (float)Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0;

            return dotProduct / (magnitude1 * magnitude2);
        }
    }
} 