using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Models.DTOs.CustomerReqestDTOs
{
    public class CreateCustomerRequestDto
    {
        public string SellerId { get; set; }
        public int? ServiceId { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
