using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaCporeReactProject.DAL.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public virtual ICollection<MovieGenry> Genry { get; set; }
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

    public class Genry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<MovieGenry> Movies { get;set;}
    }

    public class Rating
    {
        public int Id { get; set; }
        public double AverageRating { get; set; }
        public int NumberVotes { get; set; }
    }

    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
    public class Session
    {
        public int Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public Movie Movie { get; set; }
        public Cinema Cinema { get; set; }
        public int MovieId { get; set; }
    }

    public class Place
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class SessionReservation
    {
        public Place Place { get; set; }
        public Session Session { get; set; }
        [Key]
        public int SessionId { get; set; }
        [Key]
        public int PlaceId { get; set; }
        public SessionReservationStatus Status { get; set; }
    }

    public enum SessionReservationStatus
    {
        Empty = 0,
        Reserved = 1,
        Bought = 2,
    }

    public class MovieGenry
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int GenryId { get; set; }
        public Genry Genry { get; set; }
    }
}
