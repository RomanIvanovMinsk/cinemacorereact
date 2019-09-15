using CinemaCporeReactProject.DAL.Models;
using CinemaCporeReactProject.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CinemaCporeReactProject.DAL.Repositores
{
    public interface IMoviesRepository
    {
        MoviesCollectionResponse Search(SearchParameters parameters);
    }
    public class MoviesRepository : DbContext, IMoviesRepository
    {
        public MoviesRepository(DbContextOptions<MoviesRepository> options)
            : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genry> Genries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionReservation> SessionReservations { get; set; }
        public DbSet<MovieGenry> MovieGenry { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieGenry>().HasKey(x => new { x.MovieId, x.GenryId });
            modelBuilder.Entity<MovieGenry>().HasOne(x => x.Movie).WithMany(x => x.Genry).HasForeignKey(x => x.GenryId);
            modelBuilder.Entity<MovieGenry>().HasOne(x => x.Genry).WithMany(x => x.Movies).HasForeignKey(x => x.MovieId);

            modelBuilder.Entity<SessionReservation>().HasKey(x => new { x.SessionId, x.PlaceId });

            base.OnModelCreating(modelBuilder);
        }

        public void AddGenryToMovie(Movie movie, Genry genry)
        {
            MovieGenry.Add(new Models.MovieGenry()
            {
                Genry = genry,
                Movie = movie
            });
        }

        public void CreateSession(Cinema cinema, Movie movie, DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            if (start == end)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            var session = new Session()
            {
                Start = start,
                End = end,
                Cinema = cinema,
                Movie = movie
            };
            Sessions.Add(session);
        }

        public MoviesCollectionResponse Search(SearchParameters parameters)
        {
            var query = Sessions.Include(x => x.Movie).ThenInclude(x => x.Genry).Where(x => parameters.StartDate.Date >= x.Start && parameters.StartDate.Date < x.End);
            if (parameters.Genries != null && parameters.Genries.Any())
            {
                var genryIds = parameters.Genries.Select(g => int.Parse(g)).ToList();
                query = query.Where(x => MovieGenry.Any(d => genryIds.Contains(d.GenryId)));
                //query = query.Select(p=> new {p, p.Movie.Genry..Where(x => genryIds.Any(g=> x.Movie.Genry.Any(f=> f.GenryId == g)));
            }
            var mappedQuery = query.Select(x => x.Movie).Distinct().Select(x => new
            {
                Movie = x,
                Start = Sessions.Where(d => d.MovieId == x.Id && d.Start.HasValue).Min(d => d.Start),
                End = Sessions.Where(d => d.MovieId == x.Id && d.End.HasValue).Max(d => d.End)
            });
            //var mappedQuery = query.GroupBy(x => x.MovieId, x=>  new {Movie =x.Movie, Start = x.Start, End = x.End }).Select(x => new { 
            //    Movie = x.First().Movie, 
            //    Start = x.Min(s => s.Start), 
            //    End = x.Max(s => s.End) });

            var count = mappedQuery.Count();
            var items = mappedQuery.ToArray();

            var response = new MoviesCollectionResponse()
            {
                CurrentPage = parameters.Page,
                Total = items.Length,
                TotalItems = count,
                TotalPages = (items.Length - 1) / parameters.Limit + 1,
                Items = items.Select(x => new MovieModel()
                {
                    Description = x.Movie.Overview,
                    Title = x.Movie.Title,
                    Id = x.Movie.Id.ToString(),
                    Ends = x.End.Value,
                    Starts = x.Start.Value,
                    Genries = x.Movie.Genry.Select(g => new GenryModel()
                    {
                        Id = g.Genry.Id.ToString(),
                        Title = g.Genry.Title
                    }).ToArray()
                }).ToArray()
            };

            return response;
        }

        public void GetSessionsForMovie(string movieId, DateTime start = default, DateTime end = default)
        {
            if (start == default)
                start = DateTime.UtcNow.AddDays(-10);
            if (end == default)
                end = DateTime.UtcNow.AddDays(10);

            int id = int.Parse(movieId);

            var query = Sessions.Include(x => x.Movie).ThenInclude(x => x.Genry)
                .Where(x=> x.MovieId == id)
                .Where(x => start.Date >= x.Start && end.Date < x.End)
                .GroupBy(x=> x.Cinema);

        }
    }

    public class SearchParameters
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 0;
        public int Limit { get; set; } = 20;
        public string[] Genries { get; set; }
    }

    public interface ICollectionResponse<T> : IPagination
    {
        T[] Items { get; }
    }

    public interface IPagination
    {
        int CurrentPage { get; }
        int TotalPages { get; }
        int TotalItems { get; }
        int Total { get; }
    }

    public class MoviesCollectionResponse : ICollectionResponse<MovieModel>
    {
        public MovieModel[] Items { get; internal set; }
        public int CurrentPage { get; internal set; }
        public int TotalPages { get; internal set; }
        public int TotalItems { get; internal set; }
        public int Total { get; internal set; }
    }
}
