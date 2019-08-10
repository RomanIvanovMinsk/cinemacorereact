using CinemaCporeReactProject.DAL.Models.Entities;
using CinemaCporeReactProject.Models;
using CinemaCporeReactProject.Models.ReactGetStarted.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaCporeReactProject.Services
{
    public interface IUserService
    {
        Task<SResponse<UserSigninViewModel>> AuthenticateAsync(string username, string password);
        Task<SResponse<UserSigninViewModel>> CreateUser(string userName, string password);
    }
}
