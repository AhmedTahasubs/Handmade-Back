using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace Models.Domain
{
    public class CohereEmbeddingRequest
    {
        [JsonPropertyName("texts")]
        public string[] Texts { get; set; } = Array.Empty<string>();
        
        [JsonPropertyName("model")]
        public string Model { get; set; } = "embed-english-v3.0";
        
        [JsonPropertyName("input_type")]
        public string InputType { get; set; } = "search_document";
    }

    public class CohereEmbeddingResponse
    {
        [JsonPropertyName("embeddings")]
        public float[][] Embeddings { get; set; } = Array.Empty<float[]>();
        
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("texts")]
        public string[] Texts { get; set; } = Array.Empty<string>();
        
        [JsonPropertyName("meta")]
        public object? Meta { get; set; }
    }

    public class CohereEmbedder
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.cohere.ai/v1/embed";

        public CohereEmbedder(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<float[]> GetEmbeddingAsync(string text, string inputType = "search_document")
        {
            var request = new CohereEmbeddingRequest
            {
                Texts = new[] { text },
                InputType = inputType
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var embeddingResponse = JsonSerializer.Deserialize<CohereEmbeddingResponse>(responseContent);

            if (embeddingResponse?.Embeddings == null || embeddingResponse.Embeddings.Length == 0)
            {
                throw new InvalidOperationException("No embedding received from Cohere API");
            }

            return embeddingResponse.Embeddings[0];
        }

        public async Task<float[]> GetQueryEmbeddingAsync(string query)
        {
            return await GetEmbeddingAsync(query, "search_query");
        }

        public async Task<float[]> GetDocumentEmbeddingAsync(string document)
        {
            return await GetEmbeddingAsync(document, "search_document");
        }
    }
}
