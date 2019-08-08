using CinemaCporeReactProject.Models;
using CinemaCporeReactProject.Models.ReactGetStarted.Model;
using CinemaCporeReactProject.Services;
using CinemaCporeReactProject.Validators;
using CinemaCporeReactProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaCporeReactProject.Controllers
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
        public async Task<Response<UserSigninViewModel>> SignIn([FromBody]SignInRequest userParam)
        {
            Response<UserSigninViewModel> response;
            var validator = new SignInValidator();
            var validationResult = validator.Validate(userParam);
            if (!validationResult.IsValid)
            {
                response = new Response<UserSigninViewModel>();
                foreach (var error in validationResult.Errors)
                {
                    response.AddError(new Error(error.PropertyName, error.ErrorMessage, error.ErrorCode));
                }
                return response;
            }
            response = await _userService.AuthenticateAsync(userParam.Email, userParam.Password);

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<Response<UserSigninViewModel>> SignUp([FromBody]SignUpRequest userParam)
        {
            Response<UserSigninViewModel> response;
            var validator = new SignUpValidator();
            var validationResult = validator.Validate(userParam);
            if (!validationResult.IsValid)
            {
                response = new Response<UserSigninViewModel>();
                foreach (var error in validationResult.Errors)
                {
                    response.AddError(new Error(error.PropertyName, error.ErrorMessage, error.ErrorCode));
                }
                return response;
            }

            response = await _userService.CreateUser(userParam.Email, userParam.Password);

            return response;
        }


        public IActionResult TestAuth()
        {
            return Ok();
        }

    }
}