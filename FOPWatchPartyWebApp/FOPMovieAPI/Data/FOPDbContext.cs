using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace FOPMovieAPI.Data
{
    public class FOPDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; } = default!;
        public DbSet<WatchlistMovie> Watchlist { get; set; } = default!;

        public FOPDbContext(DbContextOptions<FOPDbContext> options)
            : base(options)
        {
        }
    }
}
