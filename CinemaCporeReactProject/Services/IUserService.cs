using CinemaCporeReactProject.DAL.Models.Entities;
using CinemaCporeReactProject.Models;
using CinemaCporeReactProject.Models.ReactGetStarted.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaCporeReactProject.Services
{
    public interface IUserService
    {
        Task<Response<UserSigninViewModel>> AuthenticateAsync(string username, string password);
        Task<Response<UserSigninViewModel>> CreateUser(string userName, string password);
    }
}
