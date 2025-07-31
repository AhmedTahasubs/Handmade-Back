# Content Moderation with Cohere AI - .NET Implementation

A complete content moderation solution for your .NET backend using Cohere's moderate endpoint. This implementation automatically validates product titles and descriptions before they're accepted.

## 🚀 Features

- **Automatic Product Validation**: Content moderation is integrated into product creation and updates
- **Multiple Moderation Levels**: Simple "Safe/Unsafe" and detailed category-based results
- **Batch Processing**: Process multiple texts efficiently (Admin only)
- **Comprehensive Error Handling**: Graceful degradation during API failures
- **Detailed Logging**: Full logging for monitoring and debugging
- **RESTful API**: Clean endpoints for external moderation requests

## 📋 API Endpoints

### Content Moderation Controller (`/api/ContentModeration`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/health` | Health check | No |
| `POST` | `/moderate` | Simple moderation | No |
| `POST` | `/moderate-detailed` | Detailed moderation with categories | No |
| `POST` | `/moderate-product` | Product title + description moderation | No |
| `POST` | `/moderate-batch` | Batch moderation (max 10 texts) | Admin |

### Product Controller Integration

The `ProductController` now automatically moderates content:
- **Product Creation**: Content is validated before saving
- **Product Updates**: Updated content is validated before saving
- **Detailed Feedback**: Returns specific flagged categories

## 🔧 How to Run

### 1. Build and Run
```bash
dotnet build
dotnet run --project IdentityManagerAPI
```

### 2. Test the Endpoints
Use the provided `test-content-moderation.http` file or test manually:

```bash
# Health check
curl -X GET "https://localhost:7001/api/ContentModeration/health"

# Simple moderation
curl -X POST "https://localhost:7001/api/ContentModeration/moderate" \
  -H "Content-Type: application/json" \
  -d '{"text": "Beautiful handmade wooden table"}'

# Product moderation
curl -X POST "https://localhost:7001/api/ContentModeration/moderate-product" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Handmade Wooden Coffee Table",
    "description": "Beautiful handcrafted coffee table made from premium oak wood."
  }'
```

## 📊 Example Responses

### Simple Moderation Response
```json
{
  "text": "Beautiful handmade wooden table for sale",
  "result": "Safe",
  "isSafe": true,
  "flagged": false,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Detailed Moderation Response
```json
{
  "text": "This product contains explicit adult content",
  "result": "Unsafe",
  "isSafe": false,
  "flagged": true,
  "categories": {
    "sexual": true,
    "violence": false,
    "hate": false
  },
  "categoryScores": {
    "sexual": 0.95,
    "violence": 0.1,
    "hate": 0.05
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Product Moderation Response
```json
{
  "title": "Handmade Wooden Coffee Table",
  "description": "Beautiful handcrafted coffee table...",
  "isSafe": true,
  "titleSafe": true,
  "descriptionSafe": true,
  "flaggedCategories": [],
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🛡️ Integration with Product Creation

When creating or updating products, the system automatically:

1. **Validates Content**: Checks title and description for inappropriate content
2. **Blocks Unsafe Content**: Returns error with flagged categories
3. **Proceeds if Safe**: Creates/updates product if content is appropriate

### Example Product Creation Flow
```csharp
// This happens automatically in ProductController.Post()
var moderationResult = await moderationService.ModerateProductAsync(
    productCreateDTO.Title, 
    productCreateDTO.Description
);

if (!moderationResult.IsSafe)
{
    return BadRequest(new 
    { 
        message = "Product content violates community guidelines",
        flaggedCategories = moderationResult.FlaggedCategories
    });
}

// Content is safe, proceed with creation
var productDTO = await productService.Create(productCreateDTO, userId);
```

## 🔒 Error Handling

The system handles various scenarios gracefully:

- **API Failures**: Defaults to "Safe" to avoid blocking legitimate content
- **Invalid Inputs**: Handles null, empty, or malformed content
- **Rate Limiting**: Logs warnings and continues operation
- **Network Issues**: Graceful degradation with error logging

## 📝 Configuration

The Cohere API key is configured in `appsettings.json`:

```json
{
  "Cohere": {
    "ApiKey": "KD2nozebMtBawxWruHYQe6Z2oZ4IQn4YhugsQNdU"
  }
}
```

## 🏗️ Architecture

### Core Components

1. **`CohereModerator`**: Direct API communication with Cohere
2. **`ContentModerationService`**: Business logic and logging
3. **`ContentModerationController`**: REST API endpoints
4. **`ProductController`**: Integrated moderation in product operations

### Service Registration
```csharp
// Program.cs
builder.Services.AddSingleton<CohereModerator>(provider => {
    var httpClient = provider.GetRequiredService<HttpClient>();
    var apiKey = builder.Configuration["Cohere:ApiKey"];
    return new CohereModerator(httpClient, apiKey);
});
builder.Services.AddScoped<IContentModerationService, ContentModerationService>();
```

## 🧪 Testing

### Manual Testing
Use the provided `test-content-moderation.http` file in VS Code or Postman.

### Automated Testing
```csharp
// Example unit test
[Test]
public async Task ModerateContent_WithSafeText_ReturnsSafe()
{
    var service = new ContentModerationService(mockModerator, mockLogger);
    var result = await service.ModerateContentAsync("Beautiful handmade table");
    Assert.AreEqual("Safe", result);
}
```

## 🚀 Performance Considerations

- **Caching**: Consider caching results for repeated content
- **Async Processing**: All operations are async for better performance
- **Batch Processing**: Use batch endpoints for multiple items
- **Rate Limiting**: Implement proper rate limiting for Cohere API calls

## 📞 Support

For issues and questions:
- Check the logs for detailed error information
- Verify your Cohere API key is valid
- Ensure network connectivity to Cohere API
- Review the Cohere API documentation

---

**Note**: This implementation uses Cohere's moderate endpoint. Make sure you have appropriate API access and follow Cohere's usage guidelines. 