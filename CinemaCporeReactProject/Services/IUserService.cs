using System.Collections.Generic;

namespace RomanAuthSpa.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        List<User> GetAll();
        User GetByUsername(string username, string password);     
    }
}
