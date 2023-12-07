using System;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class FopUser
    {
        [Key]
        public string Sub { get; set; }

        [Required]
        public string Name { get; set; }

        public string? PictureUrl { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
    }
}
