using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [ForeignKey("User")]
        public string SellerId { get; set; } = null!;

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        [ForeignKey("Image")]
        public int ImageId { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public Service? Servcie { get; set; }
        public Image? Image { get; set; }
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
    }
}
