using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.OrderDTO
{
    public class OrderReadDto
    {
        [Key]
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCustom { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
