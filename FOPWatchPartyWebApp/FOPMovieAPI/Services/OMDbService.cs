using ClassLibrary.Models;
using FOPMovieAPI.Controllers;
using System.Text.Json;

namespace FOPMovieAPI.Services
{
    public class OMDbService : IOMDbService
    {
        private readonly HttpClient _httpClient;

        public OMDbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Movie> GetMovieByIdAsync(string imdbID)
        {
            var response = await _httpClient.GetStringAsync($"https://www.omdbapi.com/?apikey=aaf8907d&i={imdbID}");
            return JsonSerializer.Deserialize<Movie>(response);
        }

        public async Task<Movie> GetMovieByTitleAsync(string title)
        {
            var response = await _httpClient.GetStringAsync($"https://www.omdbapi.com/?apikey=aaf8907d&type=movie&t={title}");
            return JsonSerializer.Deserialize<Movie>(response);
        }

        public async Task<Root> SearchMoviesAsync(string title)
        {
            var response = await _httpClient.GetStringAsync($"https://www.omdbapi.com/?apikey=aaf8907d&s={title}&type=movie&page=1");
            return JsonSerializer.Deserialize<Root>(response);
        }
    }
}
