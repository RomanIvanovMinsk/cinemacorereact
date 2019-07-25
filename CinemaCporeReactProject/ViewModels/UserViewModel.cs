using System.Collections.Generic;

namespace RomanAuthSpa.ViewModels
{
    internal class UserViewModel
    {
        public UserViewModel()
        {
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string UserPhoto { get; set; }

    }
}