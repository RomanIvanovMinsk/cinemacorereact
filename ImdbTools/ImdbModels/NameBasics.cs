using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImdbTools
{
    class NameBasics
    {
        [BsonId]
        public string nconst { get; set; }
        public string primaryName { get; set; }
        public string birthYear { get; set; }
        public string deathYear { get; set; }
        public string[] MyProperty { get; set; }
        public string[] knownForTitles { get; set; }
    }
}
