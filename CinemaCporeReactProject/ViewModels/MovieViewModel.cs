using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaCporeReactProject.ViewModels
{
    public class MovieViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ShortDescriptoin { get; set; }
        public double Rating { get; set; }
        public string[] Genries { get; set; }
        public DateTime End { get;set;}
    }
}
