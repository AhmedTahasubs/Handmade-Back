using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Domain
{
    public class Image
    {
        public int Id { get; set; }
        [NotMapped]
        public IFormFile File { get; set; } = null!;
		public string FileName { get; set; } = null!;
		public string FileExtension { get; set; } = null!;
		public long FileSize { get; set; }
        public string FilePath { get; set; } = null!;

	}
}
