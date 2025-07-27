using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Service
{
    public class UpdateServiceDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int DeliveryTime { get; set; }
        public string Status { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? File { get; set; }
    }
}
