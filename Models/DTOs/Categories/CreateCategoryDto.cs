using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Categories
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public IFormFile? File { get; set; }
        
    }
}
