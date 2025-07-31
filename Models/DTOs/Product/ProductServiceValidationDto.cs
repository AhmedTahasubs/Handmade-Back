using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Product
{
    public class ProductServiceValidationDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Service description must be between 10 and 1000 characters")]
        public string ServiceDescription { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Product description must be between 10 and 1000 characters")]
        public string ProductDescription { get; set; } = string.Empty;
    }

    public class ProductServiceValidationResponseDto
    {
        public string Validation { get; set; } = string.Empty; // "Yes" or "No"
        public float Confidence { get; set; }
        public string ServiceDescription { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
    }
} 