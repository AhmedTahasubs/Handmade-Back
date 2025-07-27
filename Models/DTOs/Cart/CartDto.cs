using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models.DTOs.CartItem;
namespace Models.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
