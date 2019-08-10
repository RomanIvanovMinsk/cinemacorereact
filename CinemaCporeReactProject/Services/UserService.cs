using CinemaCporeReactProject.DAL.Models.Entities;
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

namespace CinemaCporeReactProject.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;

        private readonly UserManager<IdentityUser> _service;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserService(UserManager<IdentityUser> service, SignInManager<IdentityUser> signInManager,
            IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _service = service;
            _signInManager = signInManager;
        }

        public async Task<SResponse<UserSigninViewModel>> CreateUser(string userName, string password)
        {
            var response = new SResponse<UserSigninViewModel>();
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

        public async Task<SResponse<UserSigninViewModel>> AuthenticateAsync(string username, string password)
        {
            var identity = await GetIdentity(username, password);
            var response = new SResponse<UserSigninViewModel>();
            if (identity.Key == null)
            {
                return response.AddError(new Error("Error", "Unauthorized", "user_credentials_invalid"));
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(issuer: _appSettings.Issuer,
                audience: _appSettings.Audience,
                notBefore: now,
                expires: now.Add(TimeSpan.FromDays(360)),
                signingCredentials: new SigningCredentials(_appSettings.GenerateKey(), SecurityAlgorithms.HmacSha256),
                claims: new Claim[]
                {
                    new Claim(ClaimTypes.Name, identity.Key.Id)
                });

            //// authentication successful so generate jwt token
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, identity.Key.Id)
            //    }),
            //    Expires = DateTime.UtcNow.AddDays(366),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            string strToken = new JwtSecurityTokenHandler().WriteToken(jwt);

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

    }
}
