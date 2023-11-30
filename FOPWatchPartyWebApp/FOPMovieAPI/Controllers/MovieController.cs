using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FOPMovieAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IOMDbService _omdbService;

        public MovieController(ILogger<MovieController> logger, IOMDbService omdbService)
        {
            _logger = logger;
            _omdbService = omdbService;
        }

        [HttpGet(Name = "GetMovie")]
        public async Task<ActionResult<Movie>> GetMovie()
        {
            try
            {
                var movie = await _omdbService.GetMovieDataAsync("Braveheart");
                return Ok(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie data: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
