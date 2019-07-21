using System.Collections.Generic;

namespace RomanAuthSpa.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        List<User> GetAll();
        User GetById(int parsedId, int year);     
    }
}
