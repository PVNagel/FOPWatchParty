using ClassLibrary.Models;
using FOPMovieAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace FOPMovieAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly FOPDbContext _dbContext; // Replace YourDbContext with the actual DbContext type you are using

        public UsersController(FOPDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("get-user")]
        public IActionResult GetUser(string userId)
        {
            try
            {
                var user = _dbContext.FopUsers.FirstOrDefault(u => u.Sub == userId);

                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("get-all-users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                List<FopUser> users = _dbContext.FopUsers.ToList();

                if (users != null)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound("Not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("add-user")]
        public IActionResult AddUser([FromBody] UserDataModel userData)
        {
            try
            {
                var existingUser = _dbContext.FopUsers.FirstOrDefault(u => u.Sub == userData.Sub);

                if (existingUser != null)
                {
                    return Ok("User already in database");
                }
                else
                {
                    var newUser = new FopUser
                    {
                        Sub = userData.Sub,
                        Name = userData.Name,
                        PictureUrl = userData.PictureUrl,
                        GivenName = userData.GivenName,
                        FamilyName = userData.FamilyName
                    };

                    _dbContext.FopUsers.Add(newUser);
                    _dbContext.SaveChanges();

                    return Ok("User added successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

    }

    public class UserDataModel
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
    }
}
