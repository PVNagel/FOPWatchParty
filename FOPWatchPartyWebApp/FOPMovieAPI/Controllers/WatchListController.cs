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
                var filteredWatchlist = tempWatchlist;

                if (!string.IsNullOrEmpty(year))
                {
                    filteredWatchlist = filteredWatchlist.Where(w => w.Movie.Year == year).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    filteredWatchlist = filteredWatchlist.Where(w => w.Movie.Genre.Contains(genre)).ToList();
                }

                if (!string.IsNullOrEmpty(actor))
                {
                    filteredWatchlist = filteredWatchlist.Where(w => w.Movie.Actors.Contains(actor)).ToList();
                }

                if (!string.IsNullOrEmpty(director))
                {
                    filteredWatchlist = filteredWatchlist.Where(w => w.Movie.Director == director).ToList();
                }

                var watchlist = filteredWatchlist.ToList(); // Explicitly convert to List<WatchlistMovie>

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

                if (movie == null)
                {
                    return BadRequest("Invalid IMDb ID or movie not found");
                }

                var isMovieInWatchlist = _dbContext.Watchlist.Any(w => w.Movie.imdbID == imdbID);

                if (!isMovieInWatchlist)
                {
                    var watchlistMovie = new WatchlistMovie
                    {
                        Movie = _dbContext.Movies.First(m => m.imdbID == imdbID)
                    };

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

        [HttpPost("markinterested")]
        public async Task<IActionResult> MarkInterested([FromBody] FopUserWatchlist fopUserWatchlist)
        {
            try
            {
                // Check if the user has the movie in the watchlist
                var existingWatchlistEntry = _dbContext.FopUserWatchlists.FirstOrDefault(uw => uw.Sub == fopUserWatchlist.Sub && uw.MovieId == fopUserWatchlist.MovieId);

                if (existingWatchlistEntry != null)
                {
                    // Update the IsInterested property
                    existingWatchlistEntry.IsInterested = fopUserWatchlist.IsInterested;
                }
                else
                {
                    // Create a new entry
                    var newWatchlistEntry = new FopUserWatchlist
                    {
                        Sub = fopUserWatchlist.Sub,
                        MovieId = fopUserWatchlist.MovieId,
                        IsInterested = fopUserWatchlist.IsInterested
                    };

                    // Add the new entry to the database
                    _dbContext.FopUserWatchlists.Add(newWatchlistEntry);
                }

                // Save changes to the database
                _dbContext.SaveChanges();

                return Ok("Marked as interested successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking as interested: {ex.Message}");
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
                    // Remove the movie from all FopUserWatchlists
                    var userWatchlistEntries = _dbContext.FopUserWatchlists.Where(uw => uw.MovieId == watchlistMovieToRemove.MovieId);
                    _dbContext.FopUserWatchlists.RemoveRange(userWatchlistEntries);

                    // Remove the movie from the Watchlist
                    _dbContext.Watchlist.Remove(watchlistMovieToRemove);

                    _dbContext.SaveChanges();
                    return Ok("Movie removed from watchlist and user watchlists");
                }
                else
                {
                    return NotFound("Movie not found in watchlist");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing movie from the watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpGet("getuserwatchlist")]
        public async Task<IActionResult> GetUserWatchlist(string sub)
        {
            try
            {
                // Get the user's watchlist entries
                var userWatchlist = await _dbContext.FopUserWatchlists
                    .Where(uw => uw.Sub == sub)
                    .ToListAsync();

                return Ok(userWatchlist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getuserwatchlistbymovieid")]
        public async Task<IActionResult> GetUserWatchlist(string sub, int movieId)
        {
            try
            {
                // Get the user's watchlist entries for given movie.
                var userWatchlist = await _dbContext.FopUserWatchlists
                    .Where(uw => uw.Sub == sub && uw.MovieId == movieId)
                    .ToListAsync();

                return Ok(userWatchlist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user watchlist: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<WatchlistMovie> RetrieveMovieFromDbOrApi(string imdbID)
        {
            // Check if the movie already exists in the local database
            var existingMovie = _dbContext.Movies.FirstOrDefault(m => m.imdbID == imdbID);

            if (existingMovie != null)
            {
                // Movie already exists in the database, return it
                return new WatchlistMovie { Movie = existingMovie };
            }

            // Call external API for movie
            Movie movie = await _omdbService.GetMovieByIdAsync(imdbID);

            if (movie != null)
            {
                // Movie found in the external API, add it to the local database
                _dbContext.Movies.Add(movie);
                _dbContext.SaveChanges();

                // Add the movie to the watchlist
                var watchlistMovie = new WatchlistMovie { Movie = movie };
                _dbContext.Watchlist.Add(watchlistMovie);
                _dbContext.SaveChanges();

                return watchlistMovie;
            }

            // Movie not found in both local database and external API
            return null;
        }


        [HttpGet("interesteduserscount")]
        public async Task<IActionResult> GetInterestedUsersCount([FromQuery] string imdbID)
        {
            try
            {
                // Fetch the count of interested users based on IMDb ID
                var interestedUsersCount = await CalculateInterestedUsersCount(imdbID);
                return Ok(interestedUsersCount);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching interested users count: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private async Task<int> CalculateInterestedUsersCount(string imdbID)
        {
            try
            {
                var interestedUsersCount = await _dbContext.FopUserWatchlists
                    .Where(uw => uw.Movie.imdbID == imdbID && uw.IsInterested)
                    .CountAsync();

                return interestedUsersCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating interested users count: {ex.Message}");
                throw; // Rethrow the exception for logging purposes
            }
        }
    }
}
