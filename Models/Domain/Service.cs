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

        // تفاصيل الخدمة
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int DeliveryTime { get; set; } // بالأيام
        public string Status { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
         public ICollection<Product> Products { get; set; }

        // التقييمات
        public ICollection<ServiceReview> Reviews { get; set; }


        public int? ImageId { get; set; }
        public Image Image { get; set; }
    }
}
