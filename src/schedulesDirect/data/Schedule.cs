using System.Collections.Generic;


namespace schedulesDirect.data
{
    public class Schedule
    {
        public int id { get; set; }
        public string stationID { get; set; }
        public virtual ICollection<Program> programs { get; set; }
        public Metadata metadata { get; set; }
    }
}
