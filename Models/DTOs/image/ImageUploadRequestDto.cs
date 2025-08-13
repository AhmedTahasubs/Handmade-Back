using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.image
{
    public class ImageUploadRequestDto
    {
        [Required]
        public IFormFile? ProfileImage { get; set; } = null!;
		[Required]
		public IFormFile? IdCardImage { get; set; } = null!;
	}
}
