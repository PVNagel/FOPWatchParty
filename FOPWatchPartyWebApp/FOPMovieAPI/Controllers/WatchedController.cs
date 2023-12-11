using ClassLibrary.Models;
using FOPMovieAPI.Data;
using FOPMovieAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/watched")]
    public class WatchedController : Controller
    {
        private readonly ILogger<WatchlistController> _logger;
        private readonly FOPDbContext _dbContext;
        private readonly IOMDbService _omdbService;

        public WatchedController(ILogger<WatchlistController> logger, FOPDbContext dbContext, IOMDbService omdbService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _omdbService = omdbService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetWatchedMovies()
        {
            try
            {
                List<Movie> watchedMovies = new List<Movie>();
                var tempWatched = await _dbContext.WatchedMovies.Include(w => w.Movie).ToListAsync();
                foreach (var item in tempWatched)
                {
                    var movie = item.Movie;
                    watchedMovies.Add(movie);
                }
                return Ok(watchedMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting watched list: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWatchlist(string imdbID)
        {
            try
            {
                var movie = await RetrieveMovieFromDbOrApi(imdbID);

                if (movie == null)
                {
                    return BadRequest("Invalid IMDb ID or movie not found");
                }

                var hasBeenWatched = _dbContext.WatchedMovies.Any(w => w.Movie.imdbID == imdbID);

                if (!hasBeenWatched)
                {
                    var watchedMovie = new WatchedMovie { Movie = movie ?? _dbContext.Movies.First(m => m.imdbID == imdbID) };
                    _dbContext.WatchedMovies.Add(watchedMovie);
                    _dbContext.SaveChanges();

                    return Ok("Movie added to the watched list");
                }
                else
                {
                    var watchedMovie = new WatchedMovie { Movie = movie ?? _dbContext.Movies.First(m => m.imdbID == imdbID) };
                    _dbContext.WatchedMovies.Update(watchedMovie);
                    return Ok("Movie was already in the watched list");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error adding movie to the watched list: {ex.Message}");
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
