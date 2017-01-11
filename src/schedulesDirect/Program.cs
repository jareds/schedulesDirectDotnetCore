using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using schedulesDirect.data;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace schedulesDirect
{
    public class MainProgram
    {
        public static HttpClient client = new HttpClient();
        public static string token;
        static public IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            //Get user info from ini file
            User u;
            try
            {
                u = getUserInfo();
            }
            catch (FileNotFoundException f)
            {
                Console.WriteLine("Configuration file not found, please add config.ini to the directory containing program executable");
                return;
            }
            catch (FieldAccessException f)
            {
                return;
            }
            //Delete database if exists and recreate since sqlite migrations are not good
            using (var context = new SchedulesDirectContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            initializeHttpClient(u);
            addStations(u);
            addSchedulesToDatabase();
            addProgramsToDatabase();
            using (var db = new SchedulesDirectContext())
            {
                db.Database.ExecuteSqlCommand("vacuum");
            }
        }
        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        private static void initializeHttpClient(User u)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "dotnet grabber");
            client.BaseAddress = new Uri("https://json.schedulesdirect.org/20141201/");
            //Allow use of a specific token for case when debugging by making direct rest calls and comparing to program results
            // By default every time this program runs a new token is generated and old one becomes invalid
            // This allows a token to be shared when making rest calls outside the program to avoid creating a new token after every program run
            if (u.token == null)
            {
                // No token was provided so get a new one
                var result = client.PostAsync("token", new StringContent(JsonConvert.SerializeObject(new Token { username = u.UserId, password = u.hashedPassword }))).Result;
                dynamic tokenResponse = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                token = tokenResponse.token;
            }
            else
            {
                token = u.token;
            }
            client.DefaultRequestHeaders.Add("token", token);
        }
        private static User getUserInfo()
        {
            if (!File.Exists(AppContext.BaseDirectory + "/config.ini"))
            {
                throw new FileNotFoundException();
            }
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(AppContext.BaseDirectory);
            builder.AddIniFile("config.ini");
            var config = builder.Build();
            if (config["user"] == null)
            {
                Console.WriteLine("User not found");
                throw new FieldAccessException();
            }
            else if (config["password"] == null)
            {
                Console.WriteLine("password not found");
                throw new FieldAccessException();
            }
            else if (config["country"] == null)
            {
                Console.WriteLine("Country not found");
                throw new FieldAccessException();
            }
            else if (config["zip"] == null)
            {
                Console.WriteLine("zip not found");
                throw new FieldAccessException();
            }
            User u = new User { UserId = config["user"], zip = config["zip"], country = config["country"], hashedPassword = HexStringFromBytes(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(config["password"]))) };
            // If token was not provided we will generate it
            if (config["token"] != null)
            {
                u.token = config["token"];
            }
            return u;
        }

        private static List<Lineup> getLineups(User u)
        {
            List<Lineup> lineups = new List<Lineup>();
            string country = u.country;
            string zip = u.zip;
            var result = client.GetAsync("headends?country=" + country + "&postalcode=" + zip).Result.Content;
            HeadEnd[] headends = (HeadEnd[])Newtonsoft.Json.JsonConvert.DeserializeObject(result.ReadAsStringAsync().Result, typeof(HeadEnd[]));
            foreach (HeadEnd h in headends)
            {
                foreach (Lineup l in h.lineups)
                {
                    lineups.Add(l);
                }
            }
            return lineups;
        }
        private static Lineup[] getUserLineups(User u)
        {
            dynamic lineupResponse = JObject.Parse(client.GetAsync("lineups").Result.Content.ReadAsStringAsync().Result);
            if (lineupResponse.code == 4102)
            {  //No lineups on users account, so prompt to add one
                var possibleLineups = getLineups(u);
                Console.WriteLine("Please enter the number of the lineup you want to add to your account.");
                for (int i = 0; i < possibleLineups.Count(); i++)
                {
                    Console.WriteLine("Lineup number {0} {1} {2}", i, possibleLineups[i].name, possibleLineups[i].lineup);
                }
                int lineupToAdd = Int32.Parse(Console.ReadLine());
                dynamic addLineupResponse = JObject.Parse(client.PutAsync("lineups/" + possibleLineups[lineupToAdd].lineup, null).Result.Content.ReadAsStringAsync().Result);
                if (addLineupResponse.code != 0)
                {
                    Environment.Exit(8); //Unable to add lineup so just exit
                }
                lineupResponse = JObject.Parse(client.GetAsync("lineups").Result.Content.ReadAsStringAsync().Result);
            }
            return (Lineup[])lineupResponse.lineups.ToObject(typeof(Lineup[]));
        }
        private static Station[] getStations(Lineup l)
        {
            dynamic stations = JObject.Parse(client.GetAsync("lineups/" + l.lineup).Result.Content.ReadAsStringAsync().Result);
            return (Station[])stations.stations.ToObject(typeof(Station[]));
        }

        private static Map[] getMapping(Lineup l)
        {
            dynamic mapping = JObject.Parse(client.GetAsync("lineups/" + l.lineup).Result.Content.ReadAsStringAsync().Result);
            return (Map[])mapping.map.ToObject(typeof(Map[]));
        }
        //Get stations in JSON format for use in other request
        private static String getStationIDs()
        {
            StringContent stationsString;
            using (var db = new SchedulesDirectContext())
            {
                List<dynamic> stations = new List<dynamic>();
                foreach (Station s in db.station)
                {
                    dynamic currentStation = new ExpandoObject();
                    currentStation.stationID = s.stationID;
                    stations.Add(currentStation);
                }
                stationsString = new StringContent(JsonConvert.SerializeObject(stations));
            }
            return stationsString.ReadAsStringAsync().Result;
        }
        private static void addStations(User u)
        {
            using (var db = new SchedulesDirectContext())
            {
                var lineups = getUserLineups(u);
                if (lineups == null)
                {
                    throw new Exception();
                }
                foreach (Lineup l in lineups)
                {
                    foreach (Map m in getMapping(l))
                    {
                        db.map.Add(m);
                        db.SaveChanges();
                    }
                    foreach (Station s in getStations(l))
                    {
                        db.station.Add(s);
                    }
                    db.SaveChanges();
                }
            }
        }
        private static void addSchedulesToDatabase()
        {
            using (var db = new SchedulesDirectContext())
            {
                dynamic scheduleResponse = JArray.Parse(client.PostAsync("schedules", new StringContent(getStationIDs())).Result.Content.ReadAsStringAsync().Result);
                Schedule[] schedules = (Schedule[])scheduleResponse.ToObject(typeof(Schedule[]));
                db.AddRange(schedules);
                db.SaveChanges();
            }
        }
        private static void addProgramsToDatabase()
        {
            using (var db = new SchedulesDirectContext())
            {
                List<string> allProgramIDs = new List<string>();
                foreach (Program p in db.program)
                {
                    allProgramIDs.Add(p.programID);
                }
                var programIDs = new List<string>(allProgramIDs.Distinct());
                string programString;
                List<ProgramInfo> programInfo = new List<ProgramInfo>();
                //Can only request 5000 programs at once so cycle through sending in chunks
                for (int i = 0; i < programIDs.Count(); i = i + 5000)
                {
                    if (programIDs.Count() - i > 5000)
                    {
                        programString = JsonConvert.SerializeObject(programIDs.GetRange(i, 5000));
                    }
                    else
                    {
                        programString = JsonConvert.SerializeObject(programIDs.GetRange(i, programIDs.Count() - i - 1));
                    }
                    var programDataString = client.PostAsync("programs", new StringContent(programString)).Result.Content.ReadAsStringAsync().Result;
                    programInfo.AddRange((ProgramInfo[])Newtonsoft.Json.JsonConvert.DeserializeObject(programDataString, typeof(ProgramInfo[])));
                }
                // Go through and add each genre associated with each added program
                List<Genre> genreList = new List<Genre>();
                for (int i = 0; i < programInfo.Count(); i++)
                {
                    if (programInfo[i].genres != null)
                    {
                        programInfo[i].programGenres = new List<ProgramGenre>();
                        foreach (string s in programInfo[i].genres)
                        {
                            var foundGenre = from g in genreList where g.genre == s select g;
                            Genre currentGenre;
                            if (foundGenre.Count() == 0)
                            {
                                currentGenre = new Genre { genre = s };
                                genreList.Add(currentGenre);
                            }
                            else
                            {
                                currentGenre = foundGenre.First();
                            }
                            programInfo[i].programGenres.Add(new ProgramGenre { genre = currentGenre });
                        }
                    }
                }
                db.AddRange(programInfo);
                db.SaveChanges();
            }
        }
    }
}
