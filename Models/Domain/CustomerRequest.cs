using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public static class RequestStatus
    {
        public static string Pending { get; set; } = "Pending";
        public static string Approved { get; set; } = "Approved";
        public static string Rejected { get; set; } = "Rejected";
        public static string InProgress { get; set; } = "InProgress";
        public static string Completed  { get; set; } = "Completed";
    }
    public class CustomerRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BuyerId { get; set; }

        [Required]
        public string SellerId { get; set; }

        [Required]
        public int ImageId { get; set; }

        public int? ServiceId { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ReferenceImageUrl { get; set; }

        [Required]
        public string Status { get; set; } = RequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ApplicationUser Buyer { get; set; }
        public ApplicationUser Seller { get; set; }
        public Service? Service { get; set; }
        public Image Image { get; set; }
    }
}
