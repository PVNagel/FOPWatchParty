using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class UpdateMovieRequest
    {
        public string? FopRating { get; set; }
        public string? OneOscar { get; set; }
        public string? BestQuote { get; set; }
        public string? FunniestQuote { get; set; }
        public string? CanRemakeAsNetflixSeries { get; set; }
    }
}
