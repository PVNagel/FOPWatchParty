using ClassLibrary.Models;
using FOPMovieAPI.Data;
using FOPMovieAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/watchlist")]
    public class WatchlistController : Controller
    {
        private readonly ILogger<WatchlistController> _logger;
        private readonly FOPDbContext _dbContext;
        private readonly IOMDbService _omdbService;

        public WatchlistController(ILogger<WatchlistController> logger, FOPDbContext dbContext, IOMDbService omdbService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _omdbService = omdbService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetWatchlist([FromQuery] string year = null, [FromQuery] string genre = null, [FromQuery] string actor = null, [FromQuery] string director = null)
        {
            try
            {
                // Get the initial watchlist with related movies
                var tempWatchlist = await _dbContext.Watchlist.Include(w => w.Movie).ToListAsync();

                // Apply filters
                var filteredWatchlist = tempWatchlist.Select(w => w.Movie);

                if (!string.IsNullOrEmpty(year))
                {
                    filteredWatchlist = filteredWatchlist.Where(m => m.Year == year);
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    filteredWatchlist = filteredWatchlist.Where(m => m.Genre.Contains(genre));
                }

                if (!string.IsNullOrEmpty(actor))
                {
                    filteredWatchlist = filteredWatchlist.Where(m => m.Actors.Contains(actor));
                }

                if (!string.IsNullOrEmpty(director))
                {
                    filteredWatchlist = filteredWatchlist.Where(m => m.Director == director);
                }

                var watchlist = filteredWatchlist.ToList();

                return Ok(watchlist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWatchlist(string imdbID)
        {
            try
            {
                var movie = await RetrieveMovieFromDbOrApi(imdbID);

                if(movie == null)
                {
                    return BadRequest("Invalid IMDb ID or movie not found");
                }

                var isMovieInWatchlist = _dbContext.Watchlist.Any(w => w.Movie.imdbID == imdbID);

                if (!isMovieInWatchlist)
                {
                    var watchlistMovie = new WatchlistMovie { Movie = movie ?? _dbContext.Movies.First(m => m.imdbID == imdbID) };
                    _dbContext.Watchlist.Add(watchlistMovie);
                    _dbContext.SaveChanges();

                    return Ok("Movie added to the watchlist");
                }
                else
                {
                    return Conflict("Movie is already in the watchlist");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error adding movie to the watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("remove")]
        public IActionResult RemoveFromWatchlist(string imdbID)
        {
            try
            {
                var watchlistMovieToRemove = _dbContext.Watchlist.FirstOrDefault(w => w.Movie.imdbID == imdbID);
                if (watchlistMovieToRemove != null)
                {
                    _dbContext.Watchlist.Remove(watchlistMovieToRemove);
                    _dbContext.SaveChanges();
                    return Ok("Movie removed from watchlist");
                }
                else
                {
                    return NotFound("Movie not found in watchlist");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding movie to the watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<Movie> RetrieveMovieFromDbOrApi(string imdbID)
        {

            // Check if the movie already exists in the local database
            var existingMovie = _dbContext.Movies.FirstOrDefault(m => m.imdbID == imdbID);

            if (existingMovie != null)
            {
                return existingMovie;
            }

            // Call external API for movie
            Movie movie = await _omdbService.GetMovieByIdAsync(imdbID);

            if (movie != null)
            {
                _dbContext.Movies.Add(movie);
                _dbContext.SaveChanges();
                return movie;
            }
            // Movie not found in both local database and external API
            return null;
        }
    }
}
