using ClassLibrary.Models;

namespace FOPMovieAPI.Services
{
    public interface IOMDbService
    {
        Task<Movie> GetMovieByIdDataAsync(string imdbID);
        Task<Movie> GetMovieByTitleDataAsync(string title);
        Task<Root> GetMoviesBySearchDataAsync(string title);
    }
}
