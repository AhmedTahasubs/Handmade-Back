using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class CustomerOrder
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<CustomerOrderItem> Items { get; set; } = new();
    }
}
