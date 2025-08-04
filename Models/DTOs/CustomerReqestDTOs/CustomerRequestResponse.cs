using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Domain;

namespace Models.DTOs.CustomerReqestDTOs
{
    public class CustomerRequestResponse
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public int? ServiceId { get; set; }
        public string? BuyerName { get; set; }
        public string? SellerName { get; set; }
        public string? ServiceTitle { get; set; }
        public string Description { get; set; }
        public string? ReferenceImageUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
