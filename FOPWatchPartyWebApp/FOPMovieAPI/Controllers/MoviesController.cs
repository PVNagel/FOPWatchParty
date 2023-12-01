using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ClassLibrary.OMDb_API;
using FOPMovieAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IOMDbService _omdbService;

        public MoviesController(ILogger<MoviesController> logger, IOMDbService omdbService)
        {
            _logger = logger;
            _omdbService = omdbService;
        }

        [HttpGet("movie")]
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

        [HttpGet("moviesBySearch")]
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
