using CsvHelper;
using CsvHelper.Configuration;
using LiteDB;
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
        static ILogger log;
        static async Task Main(string[] args)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://datasets.imdbws.com"),

            };


            log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt")
    .CreateLogger();
            string connectionString = @"Mode=Exclusive; Filename=MyData.db";

            db = new LiteDatabase(connectionString);
            var progressBarOptions = new ProgressBarOptions()
            {
                DisplayTimeInRealTime = true,
                EnableTaskBarProgress = true,
                ProgressCharacter = '─',
                CollapseWhenFinished = false,
            };
            try
            {
                var prog = new ProgressBar(10000, "Downloading and parsing Imdb db", progressBarOptions);
                List<Task> tasks = new List<Task>();
                tasks.Add(DownloadAndSave<TitleBasic>(connectionString, "/title.basics.tsv.gz", client, prog.Spawn(10000, "Title Basic", progressBarOptions)));
                tasks.Add(DownloadAndSave<NameBasics>(connectionString, "/name.basics.tsv.gz", client, prog.Spawn(10000, "Name Basic", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleAkas>(connectionString, "/title.akas.tsv.gz", client, prog.Spawn(10000, "Title Akas", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleCrew>(connectionString, "/title.crew.tsv.gz", client, prog.Spawn(10000, "Title Crew", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleEpicosde>(connectionString, "/title.episode.tsv.gz", client, prog.Spawn(10000, "Title Epicodes", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitlePricipals>(connectionString, "/title.principals.tsv.gz", client, prog.Spawn(10000, "Title Principals", progressBarOptions)));
                tasks.Add(DownloadAndSave<TitleRatings>(connectionString, "/title.ratings.tsv.gz", client, prog.Spawn(10000, "Title Ratings", progressBarOptions)));
                //Parallel.Invoke(
                //    async () => await DownloadAndSave<TitleBasic>(connectionString, "/title.basics.tsv.gz", client, prog.Spawn(10000, "Title Basic", progressBarOptions)),
                //    async () => await DownloadAndSave<NameBasics>(connectionString, "/name.basics.tsv.gz", client, prog.Spawn(10000, "Name Basic", progressBarOptions)),
                //    async () => await DownloadAndSave<TitleAkas>(connectionString, "/title.akas.tsv.gz", client, prog.Spawn(10000, "Title Akas", progressBarOptions))
                //    );
                await Task.WhenAll(tasks);
                prog.Dispose();
                Console.WriteLine("Complited");
                Console.ReadLine();
            }
            catch(Exception ex)
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
