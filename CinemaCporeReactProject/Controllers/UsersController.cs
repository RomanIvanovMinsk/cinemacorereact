using CinemaCporeReactProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RomanAuthSpa.Services;
using RomanAuthSpa.ViewModels;
using System.Threading.Tasks;

namespace RomanAuthSpa.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
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
        public async Task<IActionResult> SignIn([FromBody]SignInRequest userParam)
        {
            var response = await _userService.AuthenticateAsync(userParam.Email, userParam.Password);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignUp([FromBody]SignUpRequest userParam)
        {
            var response = await _userService.CreateUser(userParam.Email, userParam.Password);

            return Ok(response);
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