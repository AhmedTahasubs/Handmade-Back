using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Domain
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }
        
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; }//----------->
        public string PaymentStatus { get; set; }//------------>
        public decimal TotalPrice { get; set; }
        public bool IsCustom { get; set; }
        public DateTime OrderDate { get; set; }

        // Navigation Properties
        public ApplicationUser Buyer { get; set; }
        
        
        public List<OrderItem> OrderItems { get; set; }

    }
}
