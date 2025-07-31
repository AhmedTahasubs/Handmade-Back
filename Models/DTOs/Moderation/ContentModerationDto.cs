using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Moderation
{
    public class ContentModerationRequestDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Text must be between 1 and 1000 characters")]
        public string Text { get; set; } = string.Empty;
    }

    public class ContentModerationResponseDto
    {
        public string Text { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty; // "Safe" or "Unsafe"
        public bool IsSafe { get; set; }
        public bool Flagged { get; set; }
        public Dictionary<string, bool> Categories { get; set; } = new();
        public Dictionary<string, float> CategoryScores { get; set; } = new();
        public string? Error { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ProductModerationRequestDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        public string? SellerId { get; set; }
        public int? ProductId { get; set; }
    }

    public class ProductModerationResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSafe { get; set; }
        public bool TitleSafe { get; set; }
        public bool DescriptionSafe { get; set; }
        public List<string> FlaggedCategories { get; set; } = new();
        public string? Error { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class BatchModerationRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one text is required")]
        [MaxLength(10, ErrorMessage = "Maximum 10 texts allowed per request")]
        public string[] Texts { get; set; } = Array.Empty<string>();
    }

    public class BatchModerationResponseDto
    {
        public List<ContentModerationResponseDto> Results { get; set; } = new();
        public int Count { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
} 