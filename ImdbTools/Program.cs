using CinemaCporeReactProject.DAL.Models;
using CinemaCporeReactProject.DAL.Repositores;
using CsvHelper;
using CsvHelper.Configuration;
using LiteDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ImdbTools
{
    class Program
    {
        static LiteDatabase db;
        static LiteDatabase dbNormalized;
        static ILogger log;
        static string connectionString = @"Mode=Exclusive; Filename=MyData.db";
        static string connectionStringNormalized = @"Mode=Exclusive; Filename=MyDataNormalized.db";
        static string TheMoviesDbApiKey = "61e585b220a30d25a2f2bf597d001571";

        static async Task Main(string[] args)
        {
            db = new LiteDatabase(connectionString);
            dbNormalized = new LiteDatabase(connectionStringNormalized);
            CreateIndex();

            log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt")
    .CreateLogger();
            //await DownloadImdbDb();
            //await NormalizeDb();
            await SeedData();

            Console.ReadLine();
        }

        private static async Task SeedData()
        {
            var client = new TheMovieDbClient(TheMoviesDbApiKey);
            var genries = await client.MoviesGenries();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var dbContextBuilder = new DbContextOptionsBuilder<MoviesRepository>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"));
            var repository = new MoviesRepository(dbContextBuilder.Options);
            repository.Database.EnsureCreated();
            repository.Database.OpenConnection();
            repository.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Genries] ON");
            foreach (var genere in genries.genres)
            {
                if (repository.Genries.Any(x => x.Id == genere.id))
                    continue;
                repository.Genries.Add(new CinemaCporeReactProject.DAL.Models.Genere()
                {
                    Id = genere.id,
                    Title = genere.name
                });

            }
            repository.SaveChanges();
            repository.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Genries] OFF");

            var norlizedCollection = dbNormalized.GetCollection<MovieModel>("Movies");
            foreach (var movie in norlizedCollection.FindAll())
            {
                if (repository.Movies.Any(x=>x.ImdbId == movie.ImdbId))
                    continue;
                var movieGenries = repository.Genries.Where(x => movie.GenereIds.Contains(x.Id)).ToList();
                repository.Movies.Add(new Movie()
                {
                    ImdbId = movie.ImdbId,
                    OriginalTitle = movie.OriginalTitle,
                    Overview = movie.Overview,
                    Poster = movie.PosterUrl,
                    PosterFullUrl = movie.PosterFullUrl,
                    ReleasedDate = movie.Released,
                    Year = movie.StartYear,
                    Type = movie.TitleType,
                    Title = movie.PrimaryTitle,
                    RuntimeMinutes = movie.RuntimeMinutes,
                    Rating = new Rating()
                    {
                        AverageRating = movie.Rating.averageRating,
                        NumberVotes = movie.Rating.numVotes
                    },
                    Genere = movieGenries,
                });
            }
            repository.SaveChanges();
            Console.WriteLine("Done");
        }

        private static async Task NormalizeDb()
        {
            var collection = db.GetCollection<TitleBasic>(typeof(TitleBasic).Name);
            var ratingCollection = db.GetCollection<TitleRatings>(typeof(TitleRatings).Name);
            var norlizedCollection = dbNormalized.GetCollection<MovieModel>("Movies");
            var client = new TheMovieDbClient(TheMoviesDbApiKey);

            int count = 0;
            System.Linq.Expressions.Expression<Func<TitleBasic, bool>> predicate = x => x.startYear == "2018" && x.titleType == "movie";
            var progress = new ProgressBar(collection.Count(predicate), "ProcessingItems", new ProgressBarOptions()
            {
                EnableTaskBarProgress = true,
            });
            foreach (var item in collection.Find(predicate))
            {
                try
                {
                    var rating = ratingCollection.FindById(item.tconst);

                    if (norlizedCollection.Exists(x => x.ImdbId == item.tconst))
                        continue;
                    var info = await client.Find(item.tconst);
                    if (!info.movie_results.Any())
                    {
                        collection.Delete(item.tconst);
                        continue;
                    }
                    var entry = new MovieModel()
                    {
                        ImdbId = item.tconst,
                        EndYear = item.endYear == "\\N" ? null : (int?)int.Parse(item.endYear),
                        StartYear = item.startYear == "\\N" ? null : (int?)int.Parse(item.startYear),
                        IsAdult = item.isAdult,
                        genres = item.genres,
                        InfoDownloaded = false,
                        OriginalTitle = item.originalTitle,
                        PrimaryTitle = item.primaryTitle,
                        Rating = new MovieRating()
                        {
                            averageRating = rating?.averageRating ?? 0,
                            numVotes = rating?.numVotes ?? 0
                        },
                        RuntimeMinutes = item.runtimeMinutes == "\\N" ? null : (int?)int.Parse(item.runtimeMinutes),
                        TitleType = item.titleType,
                        Overview = info.movie_results?.FirstOrDefault()?.overview ?? "",
                        PosterUrl = info.movie_results?.FirstOrDefault()?.poster_path ?? "",
                        Released = string.IsNullOrEmpty(info.movie_results?.FirstOrDefault()?.release_date) ? null : (DateTime?)DateTime.Parse(info.movie_results?.FirstOrDefault()?.release_date),
                        PosterFullUrl = info.movie_results?.FirstOrDefault()?.FullPosterPath,
                        GenereIds = info.movie_results?.FirstOrDefault()?.genre_ids
                    };
                    norlizedCollection.Upsert(entry);
                }
                finally
                {
                    progress.Tick($"{progress.CurrentTick} of {progress.MaxTicks}");
                }
            }
        }

        private static void CreateIndex()
        {
            var collection = db.GetCollection<TitleBasic>(typeof(TitleBasic).Name);
            collection.EnsureIndex(x => x.startYear);
            collection.EnsureIndex(x => x.endYear);
            collection.EnsureIndex(x => x.titleType);

            var normalizedCollection = dbNormalized.GetCollection<MovieModel>("Movies");
            normalizedCollection.EnsureIndex(x => x.StartYear);
            normalizedCollection.EnsureIndex(x => x.EndYear);
            normalizedCollection.EnsureIndex(x => x.TitleType);
        }

        private static async Task DownloadImdbDb()
        {
            try
            {
                var client = new HttpClient()
                {
                    BaseAddress = new Uri("https://datasets.imdbws.com"),

                };


                db = new LiteDatabase(connectionString);
                var progressBarOptions = new ProgressBarOptions()
                {
                    DisplayTimeInRealTime = true,
                    EnableTaskBarProgress = true,
                    ProgressCharacter = '─',
                    CollapseWhenFinished = false,
                };

                var prog = new ProgressBar(10000, "Downloading and parsing Imdb db", progressBarOptions);
                List<Task> tasks = new List<Task>();
                tasks.Add(DownloadAndSave<TitleBasic>(connectionString, "/title.basics.tsv.gz", client, prog.Spawn(10000, "Title Basic", progressBarOptions)));
                tasks.Add(DownloadAndSave<NameBasics>(connectionString, "/name.basics.tsv.gz", client, prog.Spawn(10000, "Name Basic", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleAkas>(connectionString, "/title.akas.tsv.gz", client, prog.Spawn(10000, "Title Akas", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleCrew>(connectionString, "/title.crew.tsv.gz", client, prog.Spawn(10000, "Title Crew", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleEpicosde>(connectionString, "/title.episode.tsv.gz", client, prog.Spawn(10000, "Title Epicodes", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitlePricipals>(connectionString, "/title.principals.tsv.gz", client, prog.Spawn(10000, "Title Principals", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleRatings>(connectionString, "/title.ratings.tsv.gz", client, prog.Spawn(10000, "Title Ratings", progressBarOptions)));

                await Task.WhenAll(tasks);
                prog.Dispose();
                db.Dispose();
                db = null;
                Console.WriteLine("Complited");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Fatal");
            }
        }

        public static async Task DownloadAndSave<T>(string connectionString, string path, HttpClient client, IProgressBar progress) where T : class, new()
        {
            var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(100_000)
            {
                SingleWriter = true,
                SingleReader = false,
                FullMode = BoundedChannelFullMode.Wait
            });

            List<Task> readingTasks = new List<Task>();

            for (int i = 0; i < 2; i++)
            {
                Task readTask = WriteToDb<T>(channel.Reader, connectionString, progress);
                readingTasks.Add(readTask);
            }


            Task downloadingTask = DownloadAndParseAsync<T>(path, client, channel.Writer, progress);

            readingTasks.Add(downloadingTask);
            await Task.WhenAll(readingTasks).ConfigureAwait(false);
        }


        public static async Task DownloadAndParseAsync<T>(string url, HttpClient httpClient, ChannelWriter<T> writter, IProgressBar progress) where T : class, new()
        {
            int count = 0;
            try
            {
                using (var httpStream = await httpClient.GetStreamAsync(url))
                using (var unCompressedStream = new GZipStream(httpStream, CompressionMode.Decompress))
                using (TextReader reader = new StreamReader(unCompressedStream))
                using (var csvReader = new CsvReader(reader, new Configuration()
                {
                    Delimiter = "\t",
                    BadDataFound = (c) =>
                    {

                    },
                    IgnoreQuotes = true,
                }))
                {
                    T template = new T();
                    while (await csvReader.ReadAsync())
                    {
                        //await writter.WriteAsync(csvReader.GetRecord<T>()).ConfigureAwait(false);
                        //await Task.Delay(10);

                        await writter.WriteAsync(csvReader.GetRecord<T>()).ConfigureAwait(false);
                        count++;
                        progress.MaxTicks = count;

                    }
                    writter.Complete();

                }
            }

            catch (Exception ex)
            {
                log.Fatal(ex, "Download and parse exception");
                throw;
            }
        }


        public static async Task WriteToDb<T>(ChannelReader<T> reader, string connectionString, IProgressBar progress) where T : class
        {
            try
            {
                string baseMessage = progress.Message;
                List<T> waitingToWrite = new List<T>(1000);
                //using (var db = new LiteDatabase(connectionString))
                {
                    var col = db.GetCollection<T>(typeof(T).Name);
                    while (await reader.WaitToReadAsync())
                    {
                        var item = await reader.ReadAsync();

                        try
                        {
                            waitingToWrite.Add(item);
                            if (waitingToWrite.Count >= 1000)
                            {
                                col.Upsert(waitingToWrite);
                                waitingToWrite.Clear();
                            }
                            progress.Tick($"{baseMessage}: {progress.CurrentTick} of {progress.MaxTicks}");
                        }
                        catch (Exception ex)
                        {
                            log.Fatal(ex, "Reader exception");
                            throw;
                        }
                        finally
                        {

                        }

                    }
                    col.Upsert(waitingToWrite);
                }
                await reader.Completion;
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Reader exception");
                throw;
            }
        }
    }
}
