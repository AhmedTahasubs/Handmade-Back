namespace Models.Domain
{
    public class Service
    {
        public int Id { get; set; }

        // البائع صاحب الخدمة
        public string SellerId { get; set; }
        public ApplicationUser Seller { get; set; }

        // التصنيف
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // تفاصيل الخدمة
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int DeliveryTime { get; set; } // بالأيام
        public string Status { get; set; } = "active"; // active - paused - awiting
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // لو هنضيف مستقبلاً منتجات مرتبطة بالخدمة
        // public ICollection<Product> Products { get; set; }

        // التقييمات
        public ICollection<ServiceReview> Reviews { get; set; }


        public int? ImageId { get; set; }
        public Image Image { get; set; }
    }
}
