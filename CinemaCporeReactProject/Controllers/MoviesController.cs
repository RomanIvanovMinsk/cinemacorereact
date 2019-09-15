using CinemaCporeReactProject.DAL.Repositores;
using CinemaCporeReactProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaCporeReactProject.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MoviesController : Controller
    {
        private readonly MoviesRepository _moviesRepository;

        public MoviesController(MoviesRepository moviesRepository)
        {
            _moviesRepository = moviesRepository;
        }
    }
}
