using CinemaCporeReactProject.DAL.Repositores;
using CinemaCporeReactProject.Helpers;
using CinemaCporeReactProject.Models;
using CinemaCporeReactProject.Models.ReactGetStarted.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RomanAuthSpa.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly MoviesRepository _usersRepository;

        private readonly UserManager<IdentityUser> _service;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserService(MoviesRepository usersRepository,
            UserManager<IdentityUser> service, SignInManager<IdentityUser> signInManager)
        {
            _usersRepository = usersRepository;
            _service = service;
            _signInManager = signInManager;
        }

        public async Task<Response<UserSigninViewModel>> CreateUser(string userName, string password)
        {
            var response = new Response<UserSigninViewModel>();
            var user = await _service.FindByNameAsync(userName);
            if (user != null)
            {

                return response.AddError(new Error(nameof(userName), "User exists", "user_exists"));
            }
            var userResult = await _service.CreateAsync(new IdentityUser(userName), password);
            if (userResult.Succeeded)
            {
                return await AuthenticateAsync(userName, password);
            }
            return response.AddError(new Error("Error", "Failed to create user"));
        }

        public async Task<Response<UserSigninViewModel>> AuthenticateAsync(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            var response = new Response<UserSigninViewModel>();
            if (identity.Key == null)
            {
                return response.AddError(new Error("Error", "Unauthorized", "user_credentials_invalid"));
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identity.Key.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(366),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string strToken = tokenHandler.WriteToken(token);

            return response.Success(new UserSigninViewModel()
            {
                Email = username,
                FullName = identity.Key.UserName,
                Token = strToken
            });
        }

        private async Task<KeyValuePair<IdentityUser, IReadOnlyCollection<Claim>>> GetIdentity(string userName, string password)
        {
            List<Claim> claims = null;

            var user = await _service.FindByNameAsync(userName);
            if (user != null)
            {


                var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
                if (result.Succeeded)
                {
                    claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    };
                }
            }
            return new KeyValuePair<IdentityUser, IReadOnlyCollection<Claim>>(user, claims);
        }

        public List<User> GetAll()
        {
            return _usersRepository.Users.ToList();
        }


        public User GetByUsername(string username, string password)
        {
            User user;
            try
            {
                user = _usersRepository.Users.FirstOrDefault(x => x.Username == username && x.Password == password);
            }
            catch (Exception ex)
            {
                throw;
            }

            return user;
        }


    }
}
