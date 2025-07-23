using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.ServiceReview
{
    public class CreateServiceReviewDto
    {
        public int ServiceId { get; set; }
        public string ReviewerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
