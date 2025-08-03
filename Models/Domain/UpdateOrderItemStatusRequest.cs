using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class UpdateOrderItemStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
