using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace schedulesDirect.data
{

    public class Program
    {
        public int id { get; set; }
        public string programID { get; set; }
        public DateTime airDateTime { get; set; }
        public int duration { get; set; }
        public string md5 { get; set; }
        public bool _new { get; set; }
        [NotMapped]
        public string[] audioProperties { get; set; }
        [NotMapped]
        public virtual ICollection<Rating> ratings { get; set; }
        public bool parentalAdvisory { get; set; }
        public bool educational { get; set; }
        public string liveTapeDelay { get; set; }
        public Multipart multipart { get; set; }
        public string isPremiereOrFinale { get; set; }
        [NotMapped]
        public string[] videoProperties { get; set; }
        public bool premiere { get; set; }
        public bool dvs { get; set; }
    }
}