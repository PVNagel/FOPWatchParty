using ClassLibrary.OMDb_API;
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

        public async Task<Movie> GetMovieByTitleDataAsync(string title)
        {
            string apiUrl = $"https://www.omdbapi.com/?i=tt3896198&apikey=aaf8907d&t={title}";
            var response = await _httpClient.GetStringAsync(apiUrl);
            return JsonSerializer.Deserialize<Movie>(response);
        }
    }
}
