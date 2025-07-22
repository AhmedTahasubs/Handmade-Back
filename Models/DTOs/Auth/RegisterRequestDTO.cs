using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
		[Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
		[Required]
        public string Password { get; set; } = null!;
		[Required]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
