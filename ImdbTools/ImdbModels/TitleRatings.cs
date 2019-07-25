using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImdbTools
{
    class TitleRatings
    {
        [BsonId]
        public string tconst { get; set; }
        public double averageRating { get; set; }
        public int numVotes { get; set; }
    }
}
