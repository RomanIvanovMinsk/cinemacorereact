using CinemaCporeReactProject.DAL.Models;
using CinemaCporeReactProject.DAL.Repositores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CinemaCporeReactProject.Tests
{
    public class MoviesUnit
    {
        private MoviesRepository GetInMemoryMoviesRepository()
        {
            DbContextOptions<MoviesRepository> options;
            var builder = new DbContextOptionsBuilder<MoviesRepository>();
            builder.UseInMemoryDatabase("Movies");
            //builder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CinemaCporeReactProject_UnitTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            var factory = new LoggerFactory();
            factory.AddConsole();
            factory.AddDebug();
            builder.UseLoggerFactory(factory);
            builder.EnableSensitiveDataLogging();
            options = builder.Options;
            var context = new MoviesRepository(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        private void InitDb(MoviesRepository repository)
        {
            var genry1 = new Genry()
            {
                Title = "TestGenre1"
            };
            var genry2 = new Genry()
            {
                Title = "TestGenre2"
            };


            var movie = new Movie()
            {
                OriginalTitle = "Some Title",
                Overview = "Some overview",
                Title = "Fake Title",
                
            };
            repository.Movies.Add(movie);
            repository.AddGenryToMovie(movie, genry1);
            var movie2 = new Movie()
            {
                OriginalTitle = "Some Title",
                Overview = "Some overview",
                Title = "Fake Title",
            };
            repository.Movies.Add(movie2);
            repository.AddGenryToMovie(movie2, genry2);

            var cinema = new Cinema()
            {
                Description = "Cinema Description",
                Name = "TestCiname"
            };
            repository.Cinemas.Add(cinema);

            var now = DateTime.Now;
            repository.CreateSession(cinema, movie, now.AddDays(-10), now.AddDays(10));
            repository.CreateSession(cinema, movie, now.AddDays(-20), now.AddDays(-10));
            
            repository.SaveChanges();
        }

        [Fact]
        public void GetMoviesForToday()
        {
            //A
            MoviesRepository repository = GetInMemoryMoviesRepository();
            InitDb(repository);
            string genryId = repository.Genries.First().Id.ToString();
            var searchParameters = new SearchParameters()
            {
                StartDate = DateTime.Now,
                Genries = new[] { genryId }
            };

            //A
            var movies = repository.Search(searchParameters);

            //A
            Assert.True(movies.Items.Any());
            Assert.True(movies.Items.Length == movies.Total);
        }


        [Fact]
        public void DoNotFindMoviesInFeature()
        {
            //A
            MoviesRepository repository = GetInMemoryMoviesRepository();
            InitDb(repository);

            //A
            var movies = repository.Search(new SearchParameters()
            {
                StartDate = DateTime.Now.AddDays(10),
            });

            //A
            Assert.False(movies.Items.Any());
            Assert.True(movies.Items.Length == movies.Total);
        }
    }
}
