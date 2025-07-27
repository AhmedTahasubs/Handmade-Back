using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Service
{
    public class ServiceDto // اللي هنرجعه في الـ GET
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int DeliveryTime { get; set; }
        public string Status { get; set; }
        public string SellerName { get; set; }
        public string CategoryName { get; set; }
        public double AvgRating { get; set; }
        public string SellerId { get; set; }
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
