using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.User
{
	public class UserMangementDto
	{
		public string Id { get; set; } = null!;
		public string UserName { get; set; } = null!;
		public string FullName { get; set; } = null!;
		public DateTime CreatedOn { get; set; }	
		public string Email { get; set; } = null!;
		public bool IsDeleted { get; set; }
		public DateTime? LastUpdatedOn { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public string? NationalId { get; set; }
		public string? ProfileImageUrl { get; set; }
		public string? IdCardImageUrl { get; set; }

	}
}
