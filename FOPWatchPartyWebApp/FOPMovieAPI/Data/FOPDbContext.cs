using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FOPMovieAPI.Data
{
    public class FOPDbContext : DbContext
    {
        public DbSet<FopUser> FopUsers { get; set; } = default!;
        public DbSet<FopUserWatchlist> FopUserWatchlists { get; set; }
        public DbSet<Movie> Movies { get; set; } = default!;
        public DbSet<WatchedMovie> WatchedMovies { get; set; } = default!;
        public DbSet<WatchlistMovie> Watchlist { get; set; } = default!;

        public FOPDbContext(DbContextOptions<FOPDbContext> options)
            : base(options)
        {
        }
    }
}
