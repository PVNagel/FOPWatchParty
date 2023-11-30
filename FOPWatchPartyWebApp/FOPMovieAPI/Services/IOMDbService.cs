namespace FOPMovieAPI.Services
{
    public interface IOMDbService
    {
        Task<Movie> GetMovieByTitleDataAsync(string title);
    }
}
