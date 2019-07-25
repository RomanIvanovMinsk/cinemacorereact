using System;
using System.Collections.Generic;

namespace CinemaCporeReactProject.DAL.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public IEnumerable<Genere> Genere { get; set; }
        public string OriginalTitle { get; set; }
        public string Type { get; set; }
        public int? Year { get; set; }
        public DateTime? ReleasedDate { get; set; }
        public int? RuntimeMinutes { get; set; }
        public string Poster { get; set; }
        public string PosterFullUrl { get; set; }
        public string Overview { get; set; }
        public Rating Rating { get; set; }
    }

    public class Genere
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Rating
    {
        public int Id { get; set; }
        public double AverageRating { get; set; }
        public int NumberVotes { get; set; }
    }
}
