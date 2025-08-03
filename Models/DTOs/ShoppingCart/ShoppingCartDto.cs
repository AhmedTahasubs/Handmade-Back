using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.ShoppingCart
{
    public class ShoppingCartDto
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public List<CartItemDto> Items { get; set; } = new();
    }
}
