using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImdbTools
{
    class TitlePricipals
    {
        [BsonId]
        public string tconst { get; set; }
        public int ordering { get; set; }
        public string nconst { get; set; }
        public string category { get; set; }
        public string job { get; set; }
        public string characters { get; set; }


    }
}
