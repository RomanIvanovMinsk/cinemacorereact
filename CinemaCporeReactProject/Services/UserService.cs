using CinemaCporeReactProject.DAL.Repositores;
using CinemaCporeReactProject.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RomanAuthSpa.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private List<User> _users;
        private IHostingEnvironment _env;
		private readonly MoviesRepository _usersRepository;

		public UserService(IOptions<AppSettings> appSettings, IHostingEnvironment env, MoviesRepository usersRepository)
        {
            _appSettings = appSettings.Value;
            _env = env;
			_usersRepository = usersRepository;
		}

        public User Authenticate(string username, string password)
        {
			var user = GetByUsername(username, password);
            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
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
            catch(Exception ex)
            {
				throw;
            }

            return user;          
        }

        
    }
}
