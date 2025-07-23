namespace Models.DTOs.OrderDTO
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public int? ServiceId { get; set; }
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCustom { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
