using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }
        public ApplicationUser Buyer { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
