using CinemaCporeReactProject.Models;
using CinemaCporeReactProject.Models.ReactGetStarted.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RomanAuthSpa.Services
{
    public interface IUserService
    {
        Task<Response<UserSigninViewModel>> AuthenticateAsync(string username, string password);
        Task<Response<UserSigninViewModel>> CreateUser(string userName, string password);
        List<User> GetAll();
        User GetByUsername(string username, string password);     
    }
}
