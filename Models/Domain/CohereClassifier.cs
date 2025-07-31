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
    public class CohereClassificationRequest
    {
        [JsonPropertyName("inputs")]
        public string[] Inputs { get; set; } = Array.Empty<string>();
        
        [JsonPropertyName("examples")]
        public ClassificationExample[] Examples { get; set; } = Array.Empty<ClassificationExample>();
        
        [JsonPropertyName("model")]
        public string Model { get; set; } = "embed-english-v3.0";
    }

    public class ClassificationExample
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }

    public class CohereClassificationResponse
    {
        [JsonPropertyName("classifications")]
        public Classification[] Classifications { get; set; } = Array.Empty<Classification>();
        
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class Classification
    {
        [JsonPropertyName("input")]
        public string Input { get; set; } = string.Empty;
        
        [JsonPropertyName("prediction")]
        public string Prediction { get; set; } = string.Empty;
        
        [JsonPropertyName("confidence")]
        public float Confidence { get; set; }
        
        [JsonPropertyName("labels")]
        public Dictionary<string, float> Labels { get; set; } = new();
    }

    public class CohereClassifier
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _classifyUrl = "https://api.cohere.ai/v1/classify";
        private readonly string _generateUrl = "https://api.cohere.ai/v1/generate";

        public CohereClassifier(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<bool> ValidateProductServiceRelevanceAsync(string serviceDescription, string productDescription)
        {
            try
            {
                // Create training examples for classification
                var examples = new ClassificationExample[]
                {
                    // Positive examples - more specific and clear
                    new() { Text = "Service: Photography services for weddings and events. Product: A DSLR camera with wedding-specific lens kits.", Label = "Yes" },
                    new() { Text = "Service: Online graphic design services for logos and branding. Product: A logo design template pack.", Label = "Yes" },
                    new() { Text = "Service: Event photography for weddings and conferences. Product: A premium DSLR camera rental package.", Label = "Yes" },
                    new() { Text = "Service: Web development services. Product: Custom website template with responsive design.", Label = "Yes" },
                    new() { Text = "Service: Digital marketing consultation. Product: Marketing strategy guide and analytics tools.", Label = "Yes" },
                    new() { Text = "Service: Handmade jewelry and accessories. Product: Handcrafted silver necklace with gemstones.", Label = "Yes" },
                    new() { Text = "Service: Custom furniture making and woodworking. Product: Handmade wooden coffee table with oak finish.", Label = "Yes" },
                    new() { Text = "Service: Photography and videography. Product: Professional camera equipment and accessories.", Label = "Yes" },
                    new() { Text = "Service: Graphic design and branding. Product: Brand identity design package.", Label = "Yes" },
                    new() { Text = "Service: Web development and design. Product: E-commerce website template.", Label = "Yes" },
                    
                    // Negative examples - more diverse
                    new() { Text = "Service: Home cleaning services. Product: A handmade wooden desk.", Label = "No" },
                    new() { Text = "Service: Photography services. Product: Handmade leather wallet.", Label = "No" },
                    new() { Text = "Service: Web development. Product: Handcrafted ceramic vase.", Label = "No" },
                    new() { Text = "Service: Digital marketing. Product: Wooden furniture set.", Label = "No" },
                    new() { Text = "Service: Graphic design. Product: Handmade jewelry collection.", Label = "No" },
                    new() { Text = "Service: Photography. Product: Handmade wooden chair.", Label = "No" },
                    new() { Text = "Service: Web development. Product: Leather handbag.", Label = "No" },
                    new() { Text = "Service: Digital marketing. Product: Ceramic pottery.", Label = "No" },
                    new() { Text = "Service: Graphic design. Product: Wooden table.", Label = "No" },
                    new() { Text = "Service: Photography. Product: Handmade jewelry.", Label = "No" }
                };

                var request = new CohereClassificationRequest
                {
                    Inputs = new[] { $"Service: {serviceDescription}. Product: {productDescription}." },
                    Examples = examples
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_classifyUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Cohere API Response: {responseContent}"); // Debug log
                
                var classificationResponse = JsonSerializer.Deserialize<CohereClassificationResponse>(responseContent);

                if (classificationResponse?.Classifications == null || classificationResponse.Classifications.Length == 0)
                {
                    throw new InvalidOperationException("No classification received from Cohere API");
                }

                var classification = classificationResponse.Classifications[0];
                var prediction = classification.Prediction.ToLower();
                var confidence = classification.Confidence;

                Console.WriteLine($"Prediction: {prediction}, Confidence: {confidence}"); // Debug log

                // Return true if prediction is "yes" and confidence is above threshold
                return prediction == "yes" && confidence > 0.5f; // Lowered threshold
            }
            catch (Exception ex)
            {
                // Log the error and return false as default
                Console.WriteLine($"Cohere classification error: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetProductServiceValidationAsync(string serviceDescription, string productDescription)
        {
            var isValid = await ValidateProductServiceRelevanceAsync(serviceDescription, productDescription);
            return isValid ? "Yes" : "No";
        }

        public async Task<string> GetProductServiceValidationSimpleAsync(string serviceDescription, string productDescription)
        {
            try
            {
                var prompt = $@"You are a domain expert assistant that validates product relevance to a given service.

Your task is to evaluate whether a product logically belongs to the scope or category of a service.

Examples:
Service: Photography services for weddings and events
Product: A DSLR camera with wedding-specific lens kits
Answer: Yes

Service: Home cleaning services
Product: A handmade wooden desk
Answer: No

Service: Online graphic design services for logos and branding
Product: A logo design template pack
Answer: Yes

Service: Photography services
Product: Handmade leather wallet
Answer: No

Now evaluate this case:
Service: {serviceDescription}
Product: {productDescription}
Answer:";

                var request = new
                {
                    model = "command",
                    prompt = prompt,
                    max_tokens = 5,
                    temperature = 0.0,
                    stop_sequences = new[] { "\n", ".", " " }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_generateUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Cohere Generate Response: {responseContent}");

                // Parse the response to extract the generated text
                var generateResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var generatedText = generateResponse.GetProperty("generations")[0].GetProperty("text").GetString()?.Trim();

                Console.WriteLine($"Generated Text: '{generatedText}'");

                if (string.IsNullOrEmpty(generatedText))
                    return "No";

                // Check if the response contains "Yes" (case insensitive)
                var result = generatedText.ToLower().Contains("yes") ? "Yes" : "No";
                Console.WriteLine($"Final Result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cohere generate error: {ex.Message}");
                return "No";
            }
        }

        public async Task<string> GetProductServiceValidationFallbackAsync(string serviceDescription, string productDescription)
        {
            try
            {
                // Simple keyword-based validation as fallback
                var serviceLower = serviceDescription.ToLower();
                var productLower = productDescription.ToLower();

                // Define service categories and their related keywords
                var serviceCategories = new Dictionary<string, string[]>
                {
                    ["photography"] = new[] { "camera", "lens", "photo", "photography", "dslr", "mirrorless", "tripod", "flash", "studio" },
                    ["graphic design"] = new[] { "logo", "template", "design", "graphic", "branding", "illustration", "vector", "artwork" },
                    ["web development"] = new[] { "website", "web", "template", "code", "development", "responsive", "ecommerce", "app" },
                    ["digital marketing"] = new[] { "marketing", "strategy", "analytics", "seo", "social media", "advertising", "campaign" },
                    ["jewelry"] = new[] { "necklace", "ring", "bracelet", "earrings", "jewelry", "gemstone", "silver", "gold", "handmade" },
                    ["furniture"] = new[] { "table", "chair", "desk", "furniture", "wooden", "wood", "handmade", "custom", "coffee table" },
                    ["handmade"] = new[] { "handmade", "handcrafted", "artisan", "custom", "unique", "craft" }
                };

                // Check if any service category matches the product
                foreach (var category in serviceCategories)
                {
                    if (serviceLower.Contains(category.Key))
                    {
                        foreach (var keyword in category.Value)
                        {
                            if (productLower.Contains(keyword))
                            {
                                Console.WriteLine($"Fallback match found: Service '{category.Key}' matches product keyword '{keyword}'");
                                return "Yes";
                            }
                        }
                    }
                }

                Console.WriteLine("Fallback: No keyword match found");
                return "No";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fallback validation error: {ex.Message}");
                return "No";
            }
        }
    }
} 