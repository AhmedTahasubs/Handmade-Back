using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.Domain
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ShoppingCart")]
        public int CartId { get; set; }
        [JsonIgnore]
        public ShoppingCart Cart { get; set; } = null!;

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}