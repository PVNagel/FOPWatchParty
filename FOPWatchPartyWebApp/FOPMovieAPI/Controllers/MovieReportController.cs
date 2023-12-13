using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClassLibrary.Models;
using FOPMovieAPI.Services;
using FOPMovieAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieReportController : ControllerBase
    {
        private readonly ILogger<MovieReportController> _logger;
        private readonly FOPDbContext _dbContext;

        public MovieReportController(ILogger<MovieReportController> logger, FOPDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("getUserReports")]
        public async Task<IActionResult> GetUsersReports(string userId)
        {
            try
            {
                var movieReports = await _dbContext.MovieReports
                                                   .Where(r => r.Sub == userId)
                                                   .ToListAsync();

                return Ok(movieReports);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movies from the database: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getMovieReportsByMovieId")]
        public async Task<IActionResult> GetMovieReportsByMovieId(int movieId)
        {
            try
            {
                var movieReports = await _dbContext.MovieReports
                                                   .Where(r => r.MovieId == movieId)
                                                   .ToListAsync();

                return Ok(movieReports);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie reports by MovieId: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("getReportByMovieIdAndUserId")]
        public async Task<IActionResult> GetReportByMovieIdAndUserId(int movieId, string userId)
        {
            try
            {
                var movieReport = await _dbContext.MovieReports
                                                  .FirstOrDefaultAsync(r => r.MovieId == movieId && r.Sub == userId);

                if (movieReport == null)
                {
                    return NotFound("Movie report not found");
                }

                return Ok(movieReport);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie report by MovieId and Sub: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("updateMovieReport")]
        public async Task<IActionResult> UpdateReport([FromBody] MovieReport movieReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if a report already exists for the given user and movie
                var existingReport = await _dbContext.MovieReports
                    .FirstOrDefaultAsync(r => r.MovieId == movieReport.MovieId && r.Sub == movieReport.Sub);

                if (existingReport != null)
                {
                    existingReport.FopRating = movieReport.FopRating;
                    existingReport.OneOscar = movieReport.OneOscar;
                    existingReport.BestQuote = movieReport.BestQuote;
                    existingReport.FunniestQuote = movieReport.FunniestQuote;
                    existingReport.CanRemakeAsNetflixSeries = movieReport.CanRemakeAsNetflixSeries;
                }
                else
                {
                    return NotFound("Report not found for the given user and movie.");
                }

                await _dbContext.SaveChangesAsync();

                return Ok(existingReport); // Returning the updated report
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating movie report: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost("createMovieReport")]
        public async Task<IActionResult> CreateReport([FromBody] MovieReport movieReport)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if a report already exists for the given user and movie
                var existingReport = await _dbContext.MovieReports
                    .FirstOrDefaultAsync(r => r.MovieId == movieReport.MovieId && r.Sub == movieReport.Sub);

                if (existingReport != null)
                {
                    // Update the existing report
                    existingReport.FopRating = movieReport.FopRating;
                    existingReport.OneOscar = movieReport.OneOscar;
                    existingReport.BestQuote = movieReport.BestQuote;
                    existingReport.FunniestQuote = movieReport.FunniestQuote;
                    existingReport.CanRemakeAsNetflixSeries = movieReport.CanRemakeAsNetflixSeries;
                }
                else
                {
                    // Create a new report
                    _dbContext.MovieReports.Add(movieReport);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(movieReport);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating/updating movie report: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}