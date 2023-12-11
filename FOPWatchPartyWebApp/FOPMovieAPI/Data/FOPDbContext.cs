using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FOPMovieAPI.Data
{
    public class FOPDbContext : DbContext
    {
        public DbSet<FopUser> FopUsers { get; set; }
        public DbSet<FopUserWatchlist> FopUserWatchlists { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieReport> MovieReports { get; set; }
        public DbSet<WatchedMovie> WatchedMovies { get; set; }
        public DbSet<WatchlistMovie> Watchlist { get; set; }

        public FOPDbContext(DbContextOptions<FOPDbContext> options)
            : base(options)
        {
        }
    }
}
