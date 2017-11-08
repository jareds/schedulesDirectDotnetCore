using System.Collections.Generic;

namespace schedulesDirect.data
{

    public class Descriptions
    {
        public int id { get; set; }
        public virtual ICollection<Description1000> description1000 { get; set; }
        public virtual ICollection<Description100> description100 { get; set; }
    }
}