using ClassLibrary.Models;

namespace FOPMovieAPI.Services
{
    public interface IOMDbService
    {
        Task<Movie> GetMovieByIdAsync(string imdbID);
        Task<Movie> GetMovieByTitleAsync(string title);
        Task<Root> SearchMoviesAsync(string title);
    }
}
