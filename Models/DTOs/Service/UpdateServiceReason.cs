using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Service
{
    public class UpdateServiceReason
    {
        [Required]
        public string? Reason { get; set; }
    }
}
