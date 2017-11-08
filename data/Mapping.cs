using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace schedulesDirect.data
{

    public class LineupMetadata
    {
        public string lineup { get; set; }
        public DateTime modified { get; set; }
        public string transport { get; set; }
    }

    public class Map
    {
        public int id { get; set; }
        public string stationID { get; set; }
        public string channel { get; set; }
    }

    public class Station
    {
        [Key]
        public string stationID { get; set; }
        public string name { get; set; }
        public string callsign { get; set; }
        public string affiliate { get; set; }
        //public Broadcaster broadcaster { get; set; }
        public bool isCommercialFree { get; set; }
    }

    public class Broadcaster
    {
        public string city { get; set; }
        public string state { get; set; }
        public string postalcode { get; set; }
        public string country { get; set; }
    }

    public class Logo
    {
        public string URL { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string md5 { get; set; }
    }

}
