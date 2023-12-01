using ClassLibrary.OMDb_API;

namespace FOPMovieAPI.Services
{
    public interface IOMDbService
    {
        Task<Movie> GetMovieByTitleDataAsync(string title);
        Task<Root> GetMoviesBySearchDataAsync(string title);
    }
}
