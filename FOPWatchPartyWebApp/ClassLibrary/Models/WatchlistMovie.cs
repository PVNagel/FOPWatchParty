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
        public int Id { get; set; }
        public Movie Movie { get; set; }
    }
}
