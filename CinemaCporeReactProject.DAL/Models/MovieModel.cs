using System;

namespace CinemaCporeReactProject.DAL.Models
{
    public class MovieModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public GenryModel[] Genries { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }
    }

    public class GenryModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
