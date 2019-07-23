using Refit;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImdbTools
{
    [Headers("ContentType: application/json")]
    interface ITheMovieDbApiClient
    {
        [Get("/find/{external_id}")]
        Task<TheMovieDbResponse> Find(string external_id, string api_key, string external_source = "imdb_id", string language = "");
        [Get("/genre/movie/list")]
        Task<Generies> MoviesGeneries(string api_key, string language = "en-US");
    }

    public class TheMovieDbClient
    {
        readonly string _apiKey;
        private readonly int _requestsPerMinute = 60;
        private int _lastRequestTime;

        ITheMovieDbApiClient _client;
        public TheMovieDbClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));


            this._apiKey = apiKey;
            var httpClient = new HttpClient(new DebugHandler(new HttpClientHandler()))
            {
                BaseAddress = new Uri("https://api.themoviedb.org/3"),
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            this._client = Refit.RestService.For<ITheMovieDbApiClient>(httpClient);
        }

        public async Task<TheMovieDbResponse> Find(string externalId)
        {
            int elapsedTime = Environment.TickCount - _lastRequestTime;
            int pause = (60 / _requestsPerMinute) * 1000;
            int wait = pause - elapsedTime;
            if (wait > 0)
            {
                Thread.Sleep(wait);
            }
            var res = await _client.Find(externalId, _apiKey);
            _lastRequestTime = Environment.TickCount;
            return res;
        }

        public Task<Generies> MoviesGenries()
        {
            return _client.MoviesGeneries(_apiKey);
        }
    }

    internal class DebugHandler : DelegatingHandler
    {
        public DebugHandler(HttpMessageHandler innerHandler) :
            base(innerHandler)
        { }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Debug.WriteLine(request.RequestUri.AbsoluteUri);
            var response = await base.SendAsync(request, cancellationToken);

            Debug.WriteLine("Response:");
            Debug.WriteLine(response.ToString());
            if (response.Content != null)
            {
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
            }
            //Debug.WriteLine();

            return response;
        }
    }



    public class TheMovieDbResponse
    {
        public Movie_Results[] movie_results { get; set; }
        public object[] person_results { get; set; }
        public object[] tv_results { get; set; }
        public object[] tv_episode_results { get; set; }
        public object[] tv_season_results { get; set; }
    }

    public class Movie_Results
    {
        public bool adult { get; set; }
        public object backdrop_path { get; set; }
        public int[] genre_ids { get; set; }
        public int id { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
        public float popularity { get; set; }
        public string FullPosterPath => "http://image.tmdb.org/t/p/original" + poster_path;
    }

    public class Genere
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Generies
    {
        public Genere[] genres { get; set; }
    }

}
