using LiteDB;
using System;
using System.Collections.Generic;

namespace ImdbTools
{
    class MovieModel
    {
        public MovieModel()
        {
            Titles = new List<MovieTitles>();
            Rating = new MovieRating();
        }
        [BsonId]
        public string ImdbId { get; set; }
        public string TitleType { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public int? RuntimeMinutes { get; set; }
        public string[] genres { get; set; }

        public List<MovieTitles> Titles { get; set; }
        public MovieRating Rating { get; set; }
        public bool InfoDownloaded { get; set; }
        public string PosterUrl { get; set; }
        public string PosterFullUrl { get; set; }
        public DateTime? Released { get; set; }
        public string Overview { get; set; }
        public int[] GenereIds { get; set; }
    }

    class MovieTitles
    {
        public int Ordering { get; set; }
        public string Title { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
        public string[] Types { get; set; }
        public string[] Attributes { get; set; }
        public bool IsOriginalTitle { get; set; }
    }

    class MovieRating
    {
        public double averageRating { get; set; }
        public int numVotes { get; set; }
    }
}
