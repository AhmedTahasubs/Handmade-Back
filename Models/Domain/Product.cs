using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Const;
using System.Linq;

namespace Models.Domain
{
    public class Product
    {
        public int Id { get; set; }

        // Basic Info
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // From master branch
        public string Description { get; set; } = string.Empty;

        // Pricing & Inventory
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Status & Timestamps
        public string Status { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public string SellerId { get; set; } = null!;

        public int ServiceId { get; set; }

        public int ImageId { get; set; }

        // AI Search Embeddings
        public string? TitleEmbedding { get; set; }
        public string? DescriptionEmbedding { get; set; }
        public DateTime? EmbeddingsUpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("SellerId")]
        public ApplicationUser User { get; set; } = null!;
        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
        [ForeignKey("ImageId")]
        public Image? Image { get; set; }

        // Helper methods for embeddings
        public float[]? GetTitleEmbeddingArray()
        {
            if (string.IsNullOrEmpty(TitleEmbedding))
                return null;
            
            return TitleEmbedding.Split(',')
                .Select(x => float.Parse(x.Trim()))
                .ToArray();
        }

        public float[]? GetDescriptionEmbeddingArray()
        {
            if (string.IsNullOrEmpty(DescriptionEmbedding))
                return null;
            
            return DescriptionEmbedding.Split(',')
                .Select(x => float.Parse(x.Trim()))
                .ToArray();
        }

        public void SetTitleEmbeddingArray(float[] embedding)
        {
            TitleEmbedding = string.Join(",", embedding);
            EmbeddingsUpdatedAt = DateTime.UtcNow;
        }

        public void SetDescriptionEmbeddingArray(float[] embedding)
        {
            DescriptionEmbedding = string.Join(",", embedding);
            EmbeddingsUpdatedAt = DateTime.UtcNow;
        }
    }
}
