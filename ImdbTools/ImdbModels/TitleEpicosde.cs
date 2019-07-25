using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImdbTools
{
    class TitleEpicosde
    {
        [BsonId]
        public string tconst { get; set; }
        public string parentTconst { get; set; }
        public string seasonNumber { get; set; }
        public string episodeNumber { get; set; }

    }
}
