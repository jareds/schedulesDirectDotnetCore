using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace schedulesDirect.data
{


    public class ProgramInfo
    {
        public int id { get; set; }
        public string programID { get; set; }
        public string resourceID { get; set; }
        public virtual ICollection<Title> titles { get; set; }
        public Descriptions descriptions { get; set; }
        public string originalAirDate { get; set; }
        [NotMapped]
        public string[] genres { get; set; }
        public virtual ICollection<ProgramGenre> programGenres { get; set; }
        public virtual ICollection<ProgramMetadata> metadata { get; set; }
        public virtual ICollection<Contentrating> contentRating { get; set; }
        public virtual ICollection<Cast> cast { get; set; }
        public string entityType { get; set; }
        public string showType { get; set; }
        public bool hasImageArtwork { get; set; }
        public string md5 { get; set; }
        public string episodeTitle150 { get; set; }
        public virtual ICollection<Crew> crew { get; set; }
        public bool hasSeriesArtwork { get; set; }
    }
}