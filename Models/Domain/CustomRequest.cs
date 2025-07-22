using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class CustomRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }

        [ForeignKey("Seller")]
        public string SellerId { get; set; }

        [ForeignKey("Service")]
        public int? ServiceId { get; set; }

        public string Description { get; set; }

        [ForeignKey("ReferenceImage")]
        public int ReferenceImageId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ApplicationUser? Buyer { get; set; }

        public ApplicationUser? Seller { get; set; }

        public Service? Service { get; set; }
        public Image ReferenceImage { get; set; }

    }
}
