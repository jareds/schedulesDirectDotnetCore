using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace schedulesDirect.data
{

    public class HeadEnd
    {
        public string headend { get; set; }
        public string transport { get; set; }
        public string location { get; set; }
        public Lineup[] lineups { get; set; }
    }

    public class Lineup
    {
        public string name { get; set; }
        public string lineup { get; set; }
        public string uri { get; set; }
    }

}
