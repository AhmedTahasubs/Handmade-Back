using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.ServiceReview
{
    public class UpdateServiceReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
