using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaCporeReactProject.Controllers
{
    [Route("api/[controller]")]
    public class SampleData2Controller : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<string> WeatherForecasts()
        {
            return Summaries;
        }

        [Authorize]
        [HttpGet("[action]")]
        public string Authorize()
        {
            return "yes";
        }
    }
}
