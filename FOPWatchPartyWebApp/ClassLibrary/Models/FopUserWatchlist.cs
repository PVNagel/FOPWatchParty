using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class FopUserWatchlist
    {
        [Key]
        public int FopUserWatchlistId { get; set; }

        [Required]
        public string Sub { get; set; }
        public FopUser? FopUser { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        [Required]
        public bool IsInterested { get; set; }
    }
}
