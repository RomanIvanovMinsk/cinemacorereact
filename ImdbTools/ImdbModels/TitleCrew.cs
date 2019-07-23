using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImdbTools
{
    class TitleCrew
    {
        [BsonId]
        public string tconst { get; set; }
        public string[] directors { get; set; }
        public string[] writers { get; set; }
    }
}
