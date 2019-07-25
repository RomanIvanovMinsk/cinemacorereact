



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RomanAuthSpa.Services;
using RomanAuthSpa.ViewModels;

namespace RomanAuthSpa.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }


        [HttpGet("[action]")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll().ConvertAll((user) =>
            {
                return new UserViewModel()
                {
                    Id = user.Id,
                    ShortDescription = user.ShortDescription,
                    Description = user.Description,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };
            });
            return Ok(users);
        }



    }
}