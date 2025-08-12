using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Domain
{
    public class Service
    {
        public int Id { get; set; }

        [ForeignKey("Seller")]        
    
        
        public string SellerId { get; set; }
        public ApplicationUser Seller { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

       
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int DeliveryTime { get; set; }
        public string Status { get; set; } = "approved"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
         public ICollection<Product> Products { get; set; }

      
        public ICollection<ServiceReview> Reviews { get; set; }


        public int? ImageId { get; set; }
        public Image Image { get; set; }


        public string? Reason { get; set; }
    }
}
