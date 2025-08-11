using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class CustomerOrderItem
    {
        public int Id { get; set; }
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string SellerId { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
