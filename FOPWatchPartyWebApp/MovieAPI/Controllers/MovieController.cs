using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace MovieAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly ILogger<MovieController> _logger;
        private HttpClient _httpClient;
        private string apiKey = "aaf8907d";

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"http://www.omdbapi.com/?i=tt3896198&apikey={apiKey}");

        }

        [HttpGet("{movieTitle}")]
        public async Task<Movie> GetMovie(string movieTitle)
        {
            var movie = await _httpClient.GetFromJsonAsync<Movie>($"{_httpClient.BaseAddress}&t={movieTitle}");
            return movie;
        }

        [HttpGet("{AllMovies}")]
        public async Task GetAllMovies()
        {
            int totalBatches = 10;
            int batchSize = 25;

            for (int batchNumber = 1; batchNumber <= totalBatches; batchNumber++)
            {
                string url = $"{_httpClient.BaseAddress}{apiKey}&s=*&page={batchNumber}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // Read and process the response
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    // TODO: Process the jsonResult, e.g., display on your site or store in a database
                    Console.WriteLine(jsonResult);
                }
                else
                {
                    // Handle the error, e.g., log or display a message
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
                await Task.Delay(1000);
            }
        }
    }
}
