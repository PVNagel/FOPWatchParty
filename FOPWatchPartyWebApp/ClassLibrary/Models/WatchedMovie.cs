﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class WatchedMovie
    {
        [Key]
        public int WatchedMovieId { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
