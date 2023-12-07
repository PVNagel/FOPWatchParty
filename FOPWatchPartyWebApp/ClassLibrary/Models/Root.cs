using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class Root
    {
        public int Id { get; set; }
        public List<Search>? Search { get; set; }
        public string? totalResults { get; set; }
        public string? Response { get; set; }
    }
}

