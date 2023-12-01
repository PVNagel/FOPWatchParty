using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public string? Source { get; set; }
        public string? Value { get; set; }
    }
}