using ClassLibrary.Models;
using FOPMovieAPI.Data;
using FOPMovieAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchListController : Controller
    {
        private readonly ILogger<WatchListController> _logger;
        private readonly FOPDbContext _dbContext;
        private readonly IOMDbService _omdbService;

        public WatchListController(ILogger<WatchListController> logger, FOPDbContext dbContext, IOMDbService omdbService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _omdbService = omdbService;
        }

        [HttpGet("GetAllFromWatchList")]
        public async Task<List<Movie>> GetAllFromWatchList()
        {
            List<Movie> watchlist = new List<Movie>();
            var tempWatchlist = await _dbContext.Watchlist.Include(w => w.Movie).ToListAsync(); 
            foreach (var item in tempWatchlist)
            {
                var movie = item.Movie;
                watchlist.Add(movie);
            }
            return watchlist;
        }

        [HttpPost("AddToWatchList")]
        public async Task<IActionResult> AddToWatchList(string imdbID)
        {
            try
            {
                // Check if movie is stored in local database
                var existingMovie = _dbContext.Movies.FirstOrDefault(m => m.imdbID == imdbID);

                if (existingMovie == null)
                {
                    // Get movie from external API
                    var movie = await _omdbService.GetMovieByIdDataAsync(imdbID);

                    if (movie != null)
                    {
                        _dbContext.Movies.Add(movie);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        return BadRequest("Invalid IMDb ID or movie not found");
                    }
                }

                // Check if the movie is already in the watchlist
                var isMovieInWatchlist = _dbContext.Watchlist.Any(w => w.Movie.imdbID == imdbID);

                if (!isMovieInWatchlist)
                {
                    var watchlistMovie = new WatchlistMovie { Movie = existingMovie ?? _dbContext.Movies.First(m => m.imdbID == imdbID) };
                    _dbContext.Watchlist.Add(watchlistMovie);
                    _dbContext.SaveChanges();

                    return Ok("Movie added to the watchlist");
                }
                else
                {
                    return Ok("Movie is already in the watchlist");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding movie to the watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpDelete("DeleteFromWatchList")]
        public void DeleteFromWatchList(string imdbID)
        {
            var watchlistMovieToRemove = _dbContext.Watchlist.FirstOrDefault(w => w.Movie.imdbID == imdbID);
            if (watchlistMovieToRemove != null)
            {
                _dbContext.Watchlist.Remove(watchlistMovieToRemove);
                _dbContext.SaveChanges();
            }
        }
    }
}
