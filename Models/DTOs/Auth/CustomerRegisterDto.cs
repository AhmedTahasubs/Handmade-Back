using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Auth
{
	public class CustomerRegisterDto : RegisterRequestDTO
	{
		public bool HasWhatsApp { get; set; }
		public string? Address { get; set; }
		[Required]
		[RegularExpression("^01[0, 1, 2, 5]{1}[0-9]{8}$",ErrorMessage ="Invalid mobile number!")]
		public string MobileNumber { get; set; } = null!;
		
	}
}
