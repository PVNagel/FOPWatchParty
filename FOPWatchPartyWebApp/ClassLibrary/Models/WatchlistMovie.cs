using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class WatchlistMovie
    {
        [Key]
        public int WatchlistMovieId { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public bool IsInterested { get; set; }
        public int InterestedUsersCount { get; set; }
    }
}
