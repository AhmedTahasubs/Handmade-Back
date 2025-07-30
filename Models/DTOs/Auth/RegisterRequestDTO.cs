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
        [MinLength(3,ErrorMessage ="The length must be more than 3.")]
        [RegularExpression("^[a-zA-Z0-9-.@+]*$",ErrorMessage = "Username can only contain letters or digits")]
        public string UserName { get; set; } = null!;
        [Required]
		[MinLength(3, ErrorMessage = "The length must be more than 3.")]
		[RegularExpression("^[a-zA-Z-_ ]*$", ErrorMessage = "Only english letters are allowed.")]
        public string Name { get; set; } = null!;
		[Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
		[Required]
        [RegularExpression("(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}"
            ,ErrorMessage = "Passwords must contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least eight characters long.")]
        public string Password { get; set; } = null!;
    }
}
