using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Product
{
    public class SearchRequestDto
    {
        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Search query must be between 1 and 500 characters")]
        public string Query { get; set; } = string.Empty;

        [Range(1, 50, ErrorMessage = "Max results must be between 1 and 50")]
        public int MaxResults { get; set; } = 10;
    }
} 