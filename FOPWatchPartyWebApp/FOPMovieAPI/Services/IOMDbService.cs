namespace FOPMovieAPI.Services
{
    public interface IOMDbService
    {
        Task<Movie> GetMovieDataAsync(string title);
    }
}
