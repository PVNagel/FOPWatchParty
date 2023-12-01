using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ClassLibrary.Models;
using FOPMovieAPI.Data;
using FOPMovieAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly FOPDbContext _dbContext;
        private readonly IOMDbService _omdbService;

        public MoviesController(ILogger<MoviesController> logger, IOMDbService omdbService, FOPDbContext dbContext)
        {
            _logger = logger;
            _omdbService = omdbService;
            _dbContext = dbContext;
        }

        [HttpGet("GetAllMoviesFromDb")]
        public async Task<IActionResult> GetAllMoviesFromDb()
        {
            try
            {
                var allMovies = _dbContext.Movies.ToList();
                return Ok(allMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all movies from the database: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AddMovieToDb")]
        public async Task<IActionResult> AddMovieToDb(string imdbID)
        {
            // Check if the movie already exists in the local database
            var existingMovie = _dbContext.Movies.FirstOrDefault(m => m.imdbID == imdbID);

            if (existingMovie == null)
            {
                // Get movie from external API
                var movie = await _omdbService.GetMovieByIdDataAsync(imdbID);

                if (movie != null)
                {
                    _dbContext.Movies.Add(movie);
                    await _dbContext.SaveChangesAsync();

                    return Ok("Movie added to the database");
                }
                return NotFound("Can not add to db, movie not found in API");
            }
            else
            {
                return Ok("Movie is already in database");
            }
        }

        [HttpGet("GetMovieById")]
        public async Task<IActionResult> GetMovieById(string imdbID)
        {
            try
            {
                // Check if the movie already exists in the local database
                var existingMovie = _dbContext.Movies.FirstOrDefault(m => m.imdbID == imdbID);

                if (existingMovie != null)
                {
                    return Ok(existingMovie);
                }

                // Call external API for movie
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

                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetMovieByTitle")]
        public async Task<ActionResult<Movie>> GetMovieByTitle(string title)
        {
            try
            {
                var movie = await _omdbService.GetMovieByTitleDataAsync(title);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetMoviesBySearch")]
        public async Task<ActionResult<Root>> GetMoviesBySearch(string title)
        {
            try
            {
                var root = await _omdbService.GetMoviesBySearchDataAsync(title);
                
                if (root != null)
                {
                    return Ok(root);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
