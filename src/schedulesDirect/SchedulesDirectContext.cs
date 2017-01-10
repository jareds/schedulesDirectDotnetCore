using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using schedulesDirect.data;
using System;
using System.IO;


namespace schedulesDirect
{
    public class SchedulesDirectContext : DbContext
    {

        public DbSet<Map> map { get; set; }
        public DbSet<Station> station { get; set; }
        public DbSet<Schedule> schedule { get; set; }
        public DbSet<Program> program { get; set; }
        public DbSet<Metadata> metadata { get; set; }
        public DbSet<ProgramInfo> programInfo { get; set; }
        public DbSet<Genre> genre { get; set; }
        public DbSet<Descriptions> descriptions { get; set; }
        public DbSet<Description100> Description100 { get; set; }
        public DbSet<Description1000> Description1000 { get; set; }
        public DbSet<Metadata> metaData { get; set; }
        public DbSet<Multipart> multiPart { get; set; }
        public DbSet<Crew> crew { get; set; }
        public DbSet<Cast> cast { get; set; }
        public DbSet<ProgramGenre> programGenre { get; set; }
        public DbSet<Contentrating> Contentrating { get; set; }
        public DbSet<ProgramMetadata> ProgramMetadata { get; set; }
        public DbSet<Title> Title { get; set; }
        public DbSet<Gracenote> Gracenote { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./schedulesdirect.db");
        }
    }

}
