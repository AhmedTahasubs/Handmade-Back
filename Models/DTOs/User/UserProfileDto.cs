﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.User
{
	public class UserProfileDto
	{
		public string Id { get; set; } = null!;
		public string UserName { get; set; } = null!;
		public string FullName { get; set; } = null!;
		public DateTime CreatedOn { get; set; }
		public string Address { get; set; } = null!;
		public string Bio { get; set; } = null!;
		public string? Imageurl { get; set; } 
	}
}
