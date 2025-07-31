using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Models.Domain
{
    public class CohereClassifyRequest
    {
        [JsonPropertyName("inputs")]
        public string[] Inputs { get; set; } = Array.Empty<string>();
        
        [JsonPropertyName("examples")]
        public ClassifyExample[] Examples { get; set; } = Array.Empty<ClassifyExample>();
        
        [JsonPropertyName("model")]
        public string Model { get; set; } = "embed-english-v3.0";
    }

    public class ClassifyExample
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
        
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }

    public class CohereClassifyResponse
    {
        [JsonPropertyName("classifications")]
        public CohereClassification[] Classifications { get; set; } = Array.Empty<CohereClassification>();
        
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class CohereClassification
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

    public class CohereGenerateRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "c4ai-command-r7b-12-2024";
        
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;
        
        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 10;
        
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.0f;
        
        [JsonPropertyName("stop_sequences")]
        public string[] StopSequences { get; set; } = new[] { "\n", ".", "!" };
    }

    public class CohereGenerateResponse
    {
        [JsonPropertyName("generations")]
        public Generation[] Generations { get; set; } = Array.Empty<Generation>();
        
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    public class Generation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class ContentModerationResult
    {
        public bool IsSafe { get; set; }
        public bool Flagged { get; set; }
        public Dictionary<string, bool> Categories { get; set; } = new();
        public Dictionary<string, float> CategoryScores { get; set; } = new();
        public string? Error { get; set; }
    }

    public class ProductModerationResult
    {
        public bool IsSafe { get; set; }
        public bool TitleSafe { get; set; }
        public bool DescriptionSafe { get; set; }
        public List<string> FlaggedCategories { get; set; } = new();
        public string? Error { get; set; }
    }

    public class CohereModerator
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _classifyUrl = "https://api.cohere.ai/v1/classify";
        private readonly string _generateUrl = "https://api.cohere.ai/v1/generate";

        public CohereModerator(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        /// <summary>
        /// Simple content moderation that returns "Safe" or "Unsafe"
        /// </summary>
        /// <param name="input">Text content to moderate</param>
        /// <returns>"Safe" if content is appropriate, "Unsafe" if flagged</returns>
        public async Task<string> ModerateContentAsync(string input)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("CohereModerator: Empty input, returning Safe");
                    return "Safe"; // Default to safe for empty inputs
                }

                var result = await ModerateContentDetailedAsync(input);
                Console.WriteLine($"CohereModerator: Input: '{input.Substring(0, Math.Min(50, input.Length))}...' -> Result: {(result.IsSafe ? "Safe" : "Unsafe")}");
                return result.IsSafe ? "Safe" : "Unsafe";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CohereModerator Error: {ex.Message}");
                return "Safe"; // Default to safe during errors
            }
        }

        /// <summary>
        /// Detailed content moderation with category-specific results
        /// </summary>
        /// <param name="input">Text content to moderate</param>
        /// <returns>Detailed moderation results</returns>
        public async Task<ContentModerationResult> ModerateContentDetailedAsync(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("CohereModerator: Empty input for detailed moderation");
                    return new ContentModerationResult
                    {
                        IsSafe = true,
                        Flagged = false,
                        Categories = new Dictionary<string, bool>(),
                        CategoryScores = new Dictionary<string, float>(),
                        Error = null
                    };
                }

                // Quick keyword check for obvious drug content (Arabic and English)
                var quickDrugCheck = CheckForDrugKeywords(input);
                if (quickDrugCheck != null)
                {
                    Console.WriteLine("CohereModerator: Drug keywords detected, returning unsafe");
                    return quickDrugCheck;
                }

                // Try classification first (more reliable for moderation)
                var classificationResult = await TryClassificationAsync(input);
                if (classificationResult != null)
                {
                    return classificationResult;
                }

                // Fallback to generation if classification fails
                return await TryGenerationAsync(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CohereModerator Error in detailed moderation: {ex.Message}");
                Console.WriteLine($"CohereModerator Stack trace: {ex.StackTrace}");
                
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

        private ContentModerationResult? CheckForDrugKeywords(string input)
        {
            var lowerInput = input.ToLower();
            
            // Arabic drug keywords
            var arabicDrugKeywords = new[]
            {
                "مخدر", "حشيش", "هيروين", "كوكايين", "ماريجوانا", "قنب", "حشيشة", "بانجو",
                "اشترى مخدر", "بيع مخدر", "شراء مخدر", "مخدرات", "عقاقير"
            };
            
            // English drug keywords
            var englishDrugKeywords = new[]
            {
                "hashish", "cannabis", "marijuana", "heroin", "cocaine", "drugs", "narcotics",
                "buy drugs", "sell drugs", "purchase drugs", "illegal substances"
            };
            
            foreach (var keyword in arabicDrugKeywords.Concat(englishDrugKeywords))
            {
                if (lowerInput.Contains(keyword))
                {
                    Console.WriteLine($"CohereModerator: Drug keyword detected: '{keyword}'");
                    return new ContentModerationResult
                    {
                        IsSafe = false,
                        Flagged = true,
                        Categories = new Dictionary<string, bool> { { "drugs", true } },
                        CategoryScores = new Dictionary<string, float> { { "drugs", 0.95f } },
                        Error = null
                    };
                }
            }
            
            return null;
        }

        private async Task<ContentModerationResult?> TryClassificationAsync(string input)
        {
            try
            {
                var examples = new[]
                {
                    new ClassifyExample { Text = "Beautiful handmade wooden table for sale", Label = "safe" },
                    new ClassifyExample { Text = "Professional photography services for weddings", Label = "safe" },
                    new ClassifyExample { Text = "Handcrafted leather wallet made with love", Label = "safe" },
                    new ClassifyExample { Text = "Artisan soap and bath products", Label = "safe" },
                    new ClassifyExample { Text = "Vintage clothing and accessories", Label = "safe" },
                    new ClassifyExample { Text = "Custom jewelry design services", Label = "safe" },
                    new ClassifyExample { Text = "Home decor and furniture collection", Label = "safe" },
                    new ClassifyExample { Text = "Professional consulting services", Label = "safe" },
                    new ClassifyExample { Text = "Adult education courses", Label = "safe" },
                    new ClassifyExample { Text = "Violence prevention workshops", Label = "safe" },
                    new ClassifyExample { Text = "Explicit sexual content and adult material", Label = "unsafe" },
                    new ClassifyExample { Text = "Graphic violence and gore for sale", Label = "unsafe" },
                    new ClassifyExample { Text = "Hate speech and discrimination products", Label = "unsafe" },
                    new ClassifyExample { Text = "Illegal drugs and substances", Label = "unsafe" },
                    new ClassifyExample { Text = "Explicit nudity and pornography", Label = "unsafe" },
                    new ClassifyExample { Text = "Violence and attack weapons", Label = "unsafe" },
                    new ClassifyExample { Text = "Racist and discriminatory content", Label = "unsafe" }
                };

                var request = new CohereClassifyRequest
                {
                    Inputs = new[] { input },
                    Examples = examples,
                    Model = "embed-english-v3.0"
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"CohereModerator: Trying classification with {_classifyUrl}");
                Console.WriteLine($"CohereModerator: Request JSON: {json}");

                var response = await _httpClient.PostAsync(_classifyUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CohereModerator: Classification Response: {responseContent}");

                var classifyResponse = JsonSerializer.Deserialize<CohereClassifyResponse>(responseContent);

                if (classifyResponse?.Classifications == null || classifyResponse.Classifications.Length == 0)
                {
                    Console.WriteLine("CohereModerator: No classifications in response");
                    return null;
                }

                var classification = classifyResponse.Classifications[0];
                var isSafe = classification.Prediction.ToLower() == "safe";
                var confidence = classification.Confidence;

                Console.WriteLine($"CohereModerator: Classification - Prediction: {classification.Prediction}, Confidence: {confidence}");

                // Determine categories based on the input content
                var categories = new Dictionary<string, bool>();
                var categoryScores = new Dictionary<string, float>();

                if (!isSafe)
                {
                    var lowerInput = input.ToLower();
                    
                    if (lowerInput.Contains("sexual") || lowerInput.Contains("adult") || lowerInput.Contains("explicit") || lowerInput.Contains("porn"))
                    {
                        categories["sexual"] = true;
                        categoryScores["sexual"] = confidence;
                    }
                    
                    if (lowerInput.Contains("violence") || lowerInput.Contains("hate") || lowerInput.Contains("attack") || lowerInput.Contains("kill"))
                    {
                        categories["violence"] = true;
                        categoryScores["violence"] = confidence;
                    }
                    
                    if (lowerInput.Contains("hate") || lowerInput.Contains("discrimination") || lowerInput.Contains("racist"))
                    {
                        categories["hate"] = true;
                        categoryScores["hate"] = confidence;
                    }
                }

                return new ContentModerationResult
                {
                    IsSafe = isSafe,
                    Flagged = !isSafe,
                    Categories = categories,
                    CategoryScores = categoryScores,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CohereModerator: Classification failed: {ex.Message}");
                return null;
            }
        }

        private async Task<ContentModerationResult> TryGenerationAsync(string input)
        {
            try
            {
                // Use generate endpoint with a more balanced moderation prompt
                var prompt = $@"You are a content moderation system for an e-commerce platform. Analyze the following text and respond with only one word: 'Safe' or 'Unsafe'.

Text to analyze: ""{input}""

Guidelines:
- SAFE: Normal e-commerce content, handmade items, professional services, family-friendly content
- UNSAFE: Explicit sexual content, graphic violence, hate speech, illegal activities, drugs

Examples:
SAFE examples:
- ""Beautiful handmade wooden table for sale""
- ""Professional photography services for weddings""
- ""Handcrafted jewelry collection""
- ""Custom leather wallet made with love""
- ""Artisan soap and bath products""
- ""Vintage clothing and accessories""
- ""Home decor and furniture""
- ""Professional consulting services""

UNSAFE examples:
- ""Explicit sexual content and adult material""
- ""Graphic violence and gore""
- ""Hate speech and discrimination""
- ""Illegal drugs and substances""
- ""Explicit nudity and pornography""
- ""Buy hashish drugs""
- ""Purchase illegal substances""

Response:";

                // Try the Arabic model first, fallback to command-light if it fails
                var modelsToTry = new[] { "c4ai-command-r7b-12-2024", "command-light", "command" };
                Exception? lastException = null;

                foreach (var model in modelsToTry)
                {
                    try
                    {
                        var request = new CohereGenerateRequest
                        {
                            Model = model,
                            Prompt = prompt,
                            MaxTokens = 5,
                            Temperature = 0.0f,
                            StopSequences = new[] { "\n", ".", "!", " " }
                        };

                        var json = JsonSerializer.Serialize(request);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        Console.WriteLine($"CohereModerator: Trying generation with model '{model}' at {_generateUrl}");
                        Console.WriteLine($"CohereModerator: Request JSON: {json}");

                        var response = await _httpClient.PostAsync(_generateUrl, content);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"CohereModerator: Generation Response: {responseContent}");

                            var generateResponse = JsonSerializer.Deserialize<CohereGenerateResponse>(responseContent);

                            if (generateResponse?.Generations == null || generateResponse.Generations.Length == 0)
                            {
                                Console.WriteLine("CohereModerator: No generations in response");
                                continue;
                            }

                            var generatedText = generateResponse.Generations[0].Text.Trim().ToLower();
                            Console.WriteLine($"CohereModerator: Generated text: '{generatedText}'");

                            var isSafe = generatedText.Contains("safe");
                            var isUnsafe = generatedText.Contains("unsafe");

                            // If the model didn't give a clear response, default to safe for e-commerce content
                            if (!isSafe && !isUnsafe)
                            {
                                Console.WriteLine("CohereModerator: No clear response, defaulting to safe");
                                isSafe = true;
                            }

                            // Determine categories based on the input content
                            var categories = new Dictionary<string, bool>();
                            var categoryScores = new Dictionary<string, float>();

                            if (isUnsafe)
                            {
                                // Analyze content for specific categories
                                var lowerInput = input.ToLower();
                                
                                if (lowerInput.Contains("sexual") || lowerInput.Contains("adult") || lowerInput.Contains("explicit") || lowerInput.Contains("porn"))
                                {
                                    categories["sexual"] = true;
                                    categoryScores["sexual"] = 0.9f;
                                }
                                
                                if (lowerInput.Contains("violence") || lowerInput.Contains("hate") || lowerInput.Contains("attack") || lowerInput.Contains("kill"))
                                {
                                    categories["violence"] = true;
                                    categoryScores["violence"] = 0.8f;
                                }
                                
                                if (lowerInput.Contains("hate") || lowerInput.Contains("discrimination") || lowerInput.Contains("racist"))
                                {
                                    categories["hate"] = true;
                                    categoryScores["hate"] = 0.7f;
                                }

                                // Check for drug-related content
                                if (lowerInput.Contains("drug") || lowerInput.Contains("hashish") || lowerInput.Contains("cannabis") || 
                                    lowerInput.Contains("marijuana") || lowerInput.Contains("cocaine") || lowerInput.Contains("heroin"))
                                {
                                    categories["drugs"] = true;
                                    categoryScores["drugs"] = 0.95f;
                                }
                            }

                            Console.WriteLine($"CohereModerator: IsSafe: {isSafe}, IsUnsafe: {isUnsafe}");
                            Console.WriteLine($"CohereModerator: Categories: {JsonSerializer.Serialize(categories)}");

                            return new ContentModerationResult
                            {
                                IsSafe = isSafe,
                                Flagged = isUnsafe,
                                Categories = categories,
                                CategoryScores = categoryScores,
                                Error = null
                            };
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"CohereModerator: Model '{model}' failed with status {response.StatusCode}: {errorContent}");
                            lastException = new HttpRequestException($"Model '{model}' failed with status {response.StatusCode}: {errorContent}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"CohereModerator: Model '{model}' failed with exception: {ex.Message}");
                        lastException = ex;
                        continue;
                    }
                }

                // If all models failed, throw the last exception
                throw lastException ?? new InvalidOperationException("All generation models failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CohereModerator: All generation attempts failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Moderate a product's title and description
        /// </summary>
        /// <param name="title">Product title</param>
        /// <param name="description">Product description</param>
        /// <returns>Product moderation results</returns>
        public async Task<ProductModerationResult> ModerateProductAsync(string title, string description)
        {
            try
            {
                Console.WriteLine($"CohereModerator: Moderating product - Title: '{title}', Description: '{description.Substring(0, Math.Min(50, description.Length))}...'");

                // Moderate title and description separately
                var titleResult = await ModerateContentDetailedAsync(title ?? string.Empty);
                var descriptionResult = await ModerateContentDetailedAsync(description ?? string.Empty);

                // Collect all flagged categories
                var flaggedCategories = new List<string>();
                
                foreach (var category in titleResult.Categories)
                {
                    if (category.Value)
                    {
                        flaggedCategories.Add($"title_{category.Key}");
                        Console.WriteLine($"CohereModerator: Title flagged for category: {category.Key}");
                    }
                }
                
                foreach (var category in descriptionResult.Categories)
                {
                    if (category.Value)
                    {
                        flaggedCategories.Add($"description_{category.Key}");
                        Console.WriteLine($"CohereModerator: Description flagged for category: {category.Key}");
                    }
                }

                var isSafe = titleResult.IsSafe && descriptionResult.IsSafe;
                Console.WriteLine($"CohereModerator: Product moderation result - IsSafe: {isSafe}, TitleSafe: {titleResult.IsSafe}, DescriptionSafe: {descriptionResult.IsSafe}");

                return new ProductModerationResult
                {
                    IsSafe = isSafe,
                    TitleSafe = titleResult.IsSafe,
                    DescriptionSafe = descriptionResult.IsSafe,
                    FlaggedCategories = flaggedCategories,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CohereModerator Error moderating product: {ex.Message}");
                
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

        /// <summary>
        /// Batch moderation for multiple inputs
        /// </summary>
        /// <param name="inputs">Array of text content to moderate</param>
        /// <returns>Array of moderation results</returns>
        public async Task<ContentModerationResult[]> ModerateContentBatchAsync(string[] inputs)
        {
            var results = new List<ContentModerationResult>();
            
            foreach (var input in inputs)
            {
                var result = await ModerateContentDetailedAsync(input);
                results.Add(result);
            }
            
            return results.ToArray();
        }
    }
} 