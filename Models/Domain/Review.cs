using System.ComponentModel.DataAnnotations;

namespace Models.Domain
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
    }
}