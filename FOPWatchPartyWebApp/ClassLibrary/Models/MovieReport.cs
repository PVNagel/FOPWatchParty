using ClassLibrary.Models;
using System.ComponentModel.DataAnnotations;

public class MovieReport
{
    [Key]
    public int ReportId { get; set; }

    [Required]
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }

    [Required]
    public string Sub { get; set; }

    public string? FopRating { get; set; }
    public string? OneOscar { get; set; }
    public string? BestQuote { get; set; }
    public string? FunniestQuote { get; set; }
    public string? CanRemakeAsNetflixSeries { get; set; }
}
