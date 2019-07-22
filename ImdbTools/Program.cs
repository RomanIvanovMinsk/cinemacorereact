using CsvHelper;
using CsvHelper.Configuration;
using LiteDB;
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
        static async Task Main(string[] args)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://datasets.imdbws.com"),

            };



            string connectionString = @"Mode=Exclusive; Filename=MyData.db";

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
            //Parallel.Invoke(
            //    async () => await DownloadAndSave<TitleBasic>(connectionString, "/title.basics.tsv.gz", client, prog.Spawn(10000, "Title Basic", progressBarOptions)),
            //    async () => await DownloadAndSave<NameBasics>(connectionString, "/name.basics.tsv.gz", client, prog.Spawn(10000, "Name Basic", progressBarOptions)),
            //    async () => await DownloadAndSave<TitleAkas>(connectionString, "/title.akas.tsv.gz", client, prog.Spawn(10000, "Title Akas", progressBarOptions))
            //    );
            await Task.WhenAll(tasks);
            Console.WriteLine("Complited");
            Console.ReadLine();
        }

        public static async Task DownloadAndSave<T>(string connectionString, string path, HttpClient client, IProgressBar progress) where T : class, new()
        {
            var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(100_000)
            {
                SingleWriter = true,
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });

            List<Task> readingTasks = new List<Task>();

            for (int i = 0; i < 1; i++)
            {
                Task readTask = WriteToDb<T>(channel.Reader, connectionString, progress);
                readingTasks.Add(readTask);
            }


            Task downloadingTask = DownloadAndParseAsync<T>(path, client, channel.Writer, progress);

            readingTasks.Add(downloadingTask);
            await Task.WhenAll(readingTasks);
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
                throw;
            }
        }


        public static async Task WriteToDb<T>(ChannelReader<T> reader, string connectionString, IProgressBar progress) where T : class
        {
            string baseMessage = progress.Message;
            //using (var db = new LiteDatabase(connectionString))
            {
                var col = db.GetCollection<T>(typeof(T).Name);
                while (await reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    var item = await reader.ReadAsync().ConfigureAwait(false);

                    try
                    {
                        col.Upsert(item);
                        progress.Tick($"{baseMessage}: {progress.CurrentTick} of {progress.MaxTicks}");
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {

                    }

                }
            }
            await reader.Completion;
        }
    }
}
