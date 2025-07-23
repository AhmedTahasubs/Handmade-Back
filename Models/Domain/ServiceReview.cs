using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class ServiceReview
    {
        public int Id { get; set; }

        public string ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int Rating { get; set; } // من 1 لـ 5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
