using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UpdateProductStatusDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty;

    }
}
