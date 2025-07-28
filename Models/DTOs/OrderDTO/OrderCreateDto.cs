namespace Models.DTOs.OrderDTO
{
    public class OrderCreateDto
    {
        public string BuyerId { get; set; }
        
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCustom { get; set; }
    }
}
