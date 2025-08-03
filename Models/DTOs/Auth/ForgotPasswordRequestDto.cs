using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Auth
{
	public class ForgotPasswordRequestDto
	{
		[Required]
		public string Email { get; set; } = string.Empty;
	}
}
