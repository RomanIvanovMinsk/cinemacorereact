using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CinemaCporeReactProject.DAL.Repositores
{
    class DesignTimeMoviesRepositoryContext: IDesignTimeDbContextFactory<MoviesRepository>
    {
        public MoviesRepository CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<MoviesRepository>();
            optionsBuilder.UseSqlServer(connectionString);
            return new MoviesRepository(optionsBuilder.Options);
        }
    }
}
