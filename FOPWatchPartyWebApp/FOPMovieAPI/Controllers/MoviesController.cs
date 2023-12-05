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
    [Route("api/movies")]
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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMovies()
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

        [HttpPost("add")]
        public async Task<IActionResult> AddMovie(string imdbID)
        {
            try
            {
                var movie = await RetrieveMovieFromDbOrApi(imdbID);

                if (movie != null)
                {
                    return Ok("Movie added to the database");
                }
                else
                {
                    return Ok("Movie is already in database or could not be found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding movie to the database: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("imdb-id/{imdbID}")]
        public async Task<IActionResult> GetMovieById(string imdbID)
        {
            try
            {
                var movie = await RetrieveMovieFromDbOrApi(imdbID);

                if (movie != null)
                {
                    return Ok(movie);
                }
                else
                {
                    return BadRequest("Invalid IMDb ID or movie not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("update/{imdbID}")]
        public async Task<IActionResult> UpdateFopRating(string imdbID, [FromBody] UpdateMovieRequest request)
        {
            try
            {
                var movie = await RetrieveMovieFromDbOrApi(imdbID);

                if (movie != null)
                {
                    movie.FopRating = request.FopRating;
                    movie.OneOscar = request.OneOscar;
                    movie.BestQuote = request.BestQuote;
                    movie.FunniestQuote = request.FunniestQuote;
                    movie.CanRemakeAsNetflixSeries = request.CanRemakeAsNetflixSeries;

                    _dbContext.Movies.Update(movie);
                    _dbContext.SaveChanges();

                    return Ok(movie);
                }
                else
                {
                    return BadRequest("Invalid IMDb ID or movie not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating movie: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("title/{title}")]
        public async Task<ActionResult<Movie>> GetMovieByTitle(string title)
        {
            try
            {
                var movie = await _omdbService.GetMovieByTitleAsync(title);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("search/{title}")]
        public async Task<ActionResult<Root>> GetMoviesBySearch(string title)
        {
            try
            {
                var search = await _omdbService.SearchMoviesAsync(title);

                if (search != null)
                {
                    return Ok(search);
                }
                else
                {
                    return NotFound();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie search: {ex.Message}");
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
