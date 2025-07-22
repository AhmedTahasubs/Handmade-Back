using Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ProductDisplayDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("User")]
        public string SellerId { get; set; } = null!;
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        [ForeignKey("Image")]
        public int ImageId { get; set; }
        public Image? Image { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }
}
