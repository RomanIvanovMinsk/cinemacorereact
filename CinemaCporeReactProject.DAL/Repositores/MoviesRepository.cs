using CinemaCporeReactProject.DAL.Models;
using CinemaCporeReactProject.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaCporeReactProject.DAL.Repositores
{
    public class MoviesRepository : DbContext
    {
        public MoviesRepository(DbContextOptions<MoviesRepository> options)
            :base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genere> Genries { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
