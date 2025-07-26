using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
		[MaxLength(100)]
		public string FullName { get; set; } = null!;
		public bool IsDeleted { get; set; }

		public string? CreatedById { get; set; }

		public DateTime CreatedOn { get; set; } = DateTime.Now;

		public string? LastUpdatedById { get; set; }

		public DateTime? LastUpdatedOn { get; set; }
		public int? ImageId { get; set; }
        
        [ForeignKey("ImageId")]
        public Image? Image { get; set; }

		[MaxLength(20)]
		public string? NationalId { get; set; } 

		public bool HasWhatsApp { get; set; } = false;

		[MaxLength(500)]
		public string? Address { get; set; } 

		public bool IsBlackListed { get; set; }

		public string? Bio { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<ServiceReview> ServiceReviews { get; set; }

    }
}
