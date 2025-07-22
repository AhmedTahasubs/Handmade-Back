namespace Models.DTOs.CustomRequestDTO
{
    public class CustomRequestReadDto
    {
        public string Id { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public int? ServiceId { get; set; }
        public string Description { get; set; }
        public int ReferenceImageId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
