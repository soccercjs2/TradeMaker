using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Controllers
{
    public class ProjectionScraperController : ApiController
    {
        private const string NumberFireProjectionTable = "projection-data";
        private const string PffProjectionTable = "datatable";
        private const string FantasyProsProjectionTable = "data";

        private const string GamesPlayed = "1";
        private const string PassingYards = "5";
        private const string PassingTouchdowns = "6";
        private const string Interceptions = "7";
        private const string RushingYards = "14";
        private const string RushingTouchdowns = "15";
        private const string Receptions = "20";
        private const string ReceivingYards = "21";
        private const string ReceivingTouchdowns = "22";

        private Dictionary<string, int> Byes = new Dictionary<string, int>()
        {
            { "GB", 4 }, { "PHI", 4 },
            { "JAX", 5 }, { "KC", 5 }, { "NO", 5 }, { "SEA", 5 },
            { "MIN", 6 }, { "TB", 6 },
            { "CAR", 7 }, { "DAL", 7 },
            { "BAL", 8 }, { "LA", 8 }, { "MIA", 8 }, { "NYG", 8 }, { "PIT", 8 }, { "SF", 8 },
            { "ARI", 9 }, { "CHI", 9 }, { "CIN", 9 }, { "HOU", 9 }, { "NE", 9 }, { "WAS", 9 },
            { "BUF", 10 }, { "DET", 10 }, { "IND", 10 }, { "OAK", 10 },
            { "ATL", 11 }, { "DEN", 11 }, { "NYJ", 11 }, { "SD", 11 },
            { "CLE", 13 }, { "TEN", 13 }
        };


        private int CurrentWeek;

        // GET api/projectionscraper
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Projections Get()
        {
            Projections projections = new Projections();

            //calculate the current week
            CurrentWeek = GetCurrentWeek();

            //get full season projections
            GetSeasonQbProjections(ref projections);
            GetSeasonRbProjections(ref projections);
            GetSeasonWrProjections(ref projections);
            GetSeasonTeProjections(ref projections);

            //get season statistics
            GetSeasonStatistics(ref projections);

            //get next week projections
            GetNextWeekQbProjections(ref projections);
            GetNextWeekRbProjections(ref projections);
            GetNextWeekWrProjections(ref projections);
            GetNextWeekTeProjections(ref projections);

            //calculate rest of season statistics
            CalculateROSProjections(ref projections);

            return projections;
        }

        private void GetSeasonQbProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/qb.php?week=draft");

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in table.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //parse name and team out of player cell
                FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                //convert to nfl values
                NflConverter converter = new NflConverter(parser.Player, parser.Team);

                //set row values
                player.Id = projections.SeasonProjectionPlayers.Count + 1;
                player.Name = converter.Name;
                player.Position = "QB";
                player.NflTeam = converter.NflTeam;
                player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText) / 16;
                player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText) / 16;
                player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText) / 16;
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText) / 16;
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText) / 16;

                //add datarow to datatable
                projections.SeasonProjectionPlayers.Add(player);
            }
        }

        private void GetSeasonRbProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/rb.php?week=draft");

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in table.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //parse name and team out of player cell
                FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                //convert to nfl values
                NflConverter converter = new NflConverter(parser.Player, parser.Team);

                //set row values
                player.Id = projections.SeasonProjectionPlayers.Count + 1;
                player.Name = converter.Name;
                player.Position = "RB";
                player.NflTeam = converter.NflTeam;
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText) / 16;
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText) / 16;
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText) / 16;
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText) / 16;
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText) / 16;

                //add datarow to datatable
                projections.SeasonProjectionPlayers.Add(player);
            }
        }

        private void GetSeasonWrProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/wr.php?week=draft");

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in table.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //parse name and team out of player cell
                FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                //convert to nfl values
                NflConverter converter = new NflConverter(parser.Player, parser.Team);

                //set row values
                player.Id = projections.SeasonProjectionPlayers.Count + 1;
                player.Name = converter.Name;
                player.Position = "WR";
                player.NflTeam = converter.NflTeam;
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText) / 16;
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText) / 16;
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText) / 16;
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText) / 16;
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText) / 16;

                //add datarow to datatable
                projections.SeasonProjectionPlayers.Add(player);
            }
        }

        private void GetSeasonTeProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/te.php?week=draft");

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in table.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //parse name and team out of player cell
                FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                //convert to nfl values
                NflConverter converter = new NflConverter(parser.Player, parser.Team);

                //set row values
                player.Id = projections.SeasonProjectionPlayers.Count + 1;
                player.Name = converter.Name;
                player.Position = "TE";
                player.NflTeam = converter.NflTeam;
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[2]").InnerText) / 16;
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText) / 16;
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText) / 16;

                //add datarow to datatable
                projections.SeasonProjectionPlayers.Add(player);
            }
        }

        private void GetSeasonStatistics(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            JObject json = scraper.ScrapeJson("http://api.fantasy.nfl.com/v1/players/stats?statType=seasonStats&season=2016&format=json");
            //JObject json = scraper.ScrapeJson("http://api.fantasy.nfl.com/v1/players/stats?statType=seasonStats&season=2015&format=json");

            var players =
                from player in json["players"]
                select player;

            //loop through rows in projection table
            foreach (JObject jPlayer in players)
            {
                //create new datarow
                Player player = new Player();

                //create fantasy pro converter
                NflConverter converter = new NflConverter(jPlayer["name"].ToString(), jPlayer["teamAbbr"].ToString());

                //set row values
                player.Id = projections.StatisticsPlayers.Count + 1;
                player.Name = jPlayer["name"].ToString();
                player.NflTeam = jPlayer["teamAbbr"].ToString();
                player.Position = jPlayer["position"].ToString().ToUpper();
                player.GamesPlayed = int.Parse(IsNullStat(jPlayer["stats"][GamesPlayed]));

                if (player.Position == "QB")
                {
                    player.PassingYards = decimal.Parse(IsNullStat(jPlayer["stats"][PassingYards]));
                    player.PassingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][PassingTouchdowns]));
                    player.Interceptions = decimal.Parse(IsNullStat(jPlayer["stats"][Interceptions]));
                    player.RushingYards = decimal.Parse(IsNullStat(jPlayer["stats"][RushingYards]));
                    player.RushingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][RushingTouchdowns]));

                    //add datarow to datatable
                    projections.StatisticsPlayers.Add(player);
                }
                if (player.Position == "RB")
                {
                    player.RushingYards = decimal.Parse(IsNullStat(jPlayer["stats"][RushingYards]));
                    player.RushingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][RushingTouchdowns]));
                    player.Receptions = decimal.Parse(IsNullStat(jPlayer["stats"][Receptions]));
                    player.ReceivingYards = decimal.Parse(IsNullStat(jPlayer["stats"][ReceivingYards]));
                    player.ReceivingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][ReceivingTouchdowns]));

                    //add datarow to datatable
                    projections.StatisticsPlayers.Add(player);
                }
                if (player.Position == "WR")
                {
                    player.RushingYards = decimal.Parse(IsNullStat(jPlayer["stats"][RushingYards]));
                    player.RushingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][RushingTouchdowns]));
                    player.Receptions = decimal.Parse(IsNullStat(jPlayer["stats"][Receptions]));
                    player.ReceivingYards = decimal.Parse(IsNullStat(jPlayer["stats"][ReceivingYards]));
                    player.ReceivingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][ReceivingTouchdowns]));

                    //add datarow to datatable
                    projections.StatisticsPlayers.Add(player);
                }
                if (player.Position == "TE")
                {
                    player.Receptions = decimal.Parse(IsNullStat(jPlayer["stats"][Receptions]));
                    player.ReceivingYards = decimal.Parse(IsNullStat(jPlayer["stats"][ReceivingYards]));
                    player.ReceivingTouchdowns = decimal.Parse(IsNullStat(jPlayer["stats"][ReceivingTouchdowns]));

                    //add datarow to datatable
                    projections.StatisticsPlayers.Add(player);
                }
            }
        }

        private void GetNextWeekQbProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/qb.php?week=" + CurrentWeek);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //create new datarow
                    Player player = new Player();

                    //parse name and team out of player cell
                    FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                    //set row values
                    player.Id = projections.WeekProjectionPlayers.Count + 1;
                    player.Name = parser.Player;
                    player.Position = "QB";
                    player.NflTeam = parser.Team;
                    player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                    player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                    player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                    player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                    player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);

                    //add datarow to datatable
                    projections.WeekProjectionPlayers.Add(player);
                }
            }
        }

        private void GetNextWeekRbProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/rb.php?week=" + CurrentWeek);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //create new datarow
                    Player player = new Player();

                    //parse name and team out of player cell
                    FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                    //set row values
                    player.Id = projections.WeekProjectionPlayers.Count + 1;
                    player.Name = parser.Player;
                    player.Position = "RB";
                    player.NflTeam = parser.Team;
                    player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);

                    //add datarow to datatable
                    projections.WeekProjectionPlayers.Add(player);
                }
            }
        }

        private void GetNextWeekWrProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/wr.php?week=" + CurrentWeek);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //create new datarow
                    Player player = new Player();

                    //parse name and team out of player cell
                    FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                    //set row values
                    player.Id = projections.WeekProjectionPlayers.Count + 1;
                    player.Name = parser.Player;
                    player.Position = "WR";
                    player.NflTeam = parser.Team;
                    player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);

                    //add datarow to datatable
                    projections.WeekProjectionPlayers.Add(player);
                }
            }
        }

        private void GetNextWeekTeProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/te.php?week=" + CurrentWeek);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //create new datarow
                    Player player = new Player();

                    //parse name and team out of player cell
                    FantasyProsParser parser = new FantasyProsParser(row.SelectSingleNode("./td[1]"));

                    //set row values
                    player.Id = projections.WeekProjectionPlayers.Count + 1;
                    player.Name = parser.Player;
                    player.Position = "TE";
                    player.NflTeam = parser.Team;
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[2]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);

                    //add datarow to datatable
                    projections.WeekProjectionPlayers.Add(player);
                }
            }
        }

        private void CalculateROSProjections(ref Projections projections)
        {
            //make copies of players to keep track of which players have been matched
            HashSet<Player> seasonProjectionPlayers = new HashSet<Player>(projections.SeasonProjectionPlayers);
            HashSet<Player> statisticsPlayers = new HashSet<Player>(projections.StatisticsPlayers);
            HashSet<Player> weekProjectionPlayers = new HashSet<Player>(projections.WeekProjectionPlayers);

            CalculateROSProjectionsFromSeasonProjections(ref projections, ref seasonProjectionPlayers, ref statisticsPlayers, ref weekProjectionPlayers);
            CalculateROSProjectionsFromStatistics(ref projections, ref statisticsPlayers, ref weekProjectionPlayers);
        }

        private void CalculateROSProjectionsFromSeasonProjections(ref Projections projections, 
            ref HashSet<Player> seasonProjectionPlayers, ref HashSet<Player> statisticsPlayers, ref HashSet<Player> weekProjectionPlayers)
        {
            foreach (Player player in seasonProjectionPlayers)
            {
                Player match = new Player();

                //set player information
                match.Id = projections.Players.Count + 1;
                match.Name = player.Name;
                match.Position = player.Position;
                match.NflTeam = player.NflTeam;

                //find matches in statistics and week projections
                Player statisticPlayer = statisticsPlayers.Where(p => p.NflTeam == player.NflTeam && p.Position == player.Position && p.Name == player.Name).FirstOrDefault<Player>();
                Player weekProjectionPlayer = weekProjectionPlayers.Where(p => p.NflTeam == player.NflTeam && p.Position == player.Position && p.Name == player.Name).FirstOrDefault<Player>();

                //calculate weights
                int gamesPlayed = (statisticPlayer == null) ? 0 : statisticPlayer.GamesPlayed;
                int gamesProjected = (weekProjectionPlayer == null) ? 0 : 1;
                int weightSeasonProjection = (CurrentWeek < 8) ? Math.Max(16 - (2 * (gamesPlayed + gamesProjected)), 0) : 0;
                int weightInSeason = (weightSeasonProjection > 0) ? 2 : 1;
                int divisor = (weightSeasonProjection > 0) ? 16 : (gamesPlayed + gamesProjected);

                //add weighted season projections
                match.PassingYards += player.PassingYards * weightSeasonProjection;
                match.PassingTouchdowns += player.PassingTouchdowns * weightSeasonProjection;
                match.Interceptions += player.Interceptions * weightSeasonProjection;
                match.RushingYards += player.RushingYards * weightSeasonProjection;
                match.RushingTouchdowns += player.RushingTouchdowns * weightSeasonProjection;
                match.Receptions += player.Receptions * weightSeasonProjection;
                match.ReceivingYards += player.ReceivingYards * weightSeasonProjection;
                match.ReceivingTouchdowns += player.ReceivingTouchdowns * weightSeasonProjection;

                //add weighted statistics
                if (statisticPlayer != null)
                {
                    //add stats
                    match.PassingYards += statisticPlayer.PassingYards * weightInSeason;
                    match.PassingTouchdowns += statisticPlayer.PassingTouchdowns * weightInSeason;
                    match.Interceptions += statisticPlayer.Interceptions * weightInSeason;
                    match.RushingYards += statisticPlayer.RushingYards * weightInSeason;
                    match.RushingTouchdowns += statisticPlayer.RushingTouchdowns * weightInSeason;
                    match.Receptions += statisticPlayer.Receptions * weightInSeason;
                    match.ReceivingYards += statisticPlayer.ReceivingYards * weightInSeason;
                    match.ReceivingTouchdowns += statisticPlayer.ReceivingTouchdowns * weightInSeason;

                    //remove player for faster looping later
                    statisticsPlayers.Remove(statisticPlayer);
                }

                if (weekProjectionPlayer != null)
                {
                    //add stats
                    match.PassingYards += weekProjectionPlayer.PassingYards * weightInSeason;
                    match.PassingTouchdowns += weekProjectionPlayer.PassingTouchdowns * weightInSeason;
                    match.Interceptions += weekProjectionPlayer.Interceptions * weightInSeason;
                    match.RushingYards += weekProjectionPlayer.RushingYards * weightInSeason;
                    match.RushingTouchdowns += weekProjectionPlayer.RushingTouchdowns * weightInSeason;
                    match.Receptions += weekProjectionPlayer.Receptions * weightInSeason;
                    match.ReceivingYards += weekProjectionPlayer.ReceivingYards * weightInSeason;
                    match.ReceivingTouchdowns += weekProjectionPlayer.ReceivingTouchdowns * weightInSeason;

                    //remove player for faster looping later
                    weekProjectionPlayers.Remove(weekProjectionPlayer);
                }

                int bye = (match.NflTeam == null) ? 17 : Byes[match.NflTeam];
                int gamesRemaining = 17 - CurrentWeek + ((bye >= CurrentWeek) ? 0 : 1);

                //turn combination of stats into ROS projections
                match.PassingYards = match.PassingYards * gamesRemaining / divisor;
                match.PassingTouchdowns = match.PassingTouchdowns * gamesRemaining / divisor;
                match.Interceptions = match.Interceptions * gamesRemaining / divisor;
                match.RushingYards = match.RushingYards * gamesRemaining / divisor;
                match.RushingTouchdowns = match.RushingTouchdowns * gamesRemaining / divisor;
                match.Receptions = match.Receptions * gamesRemaining / divisor;
                match.ReceivingYards = match.ReceivingYards * gamesRemaining / divisor;
                match.ReceivingTouchdowns = match.ReceivingTouchdowns * gamesRemaining / divisor;

                //add match to players
                projections.Players.Add(match);
            }
        }

        private void CalculateROSProjectionsFromStatistics(ref Projections projections,
            ref HashSet<Player> statisticsPlayers, ref HashSet<Player> weekProjectionPlayers)
        {
            foreach (Player player in statisticsPlayers)
            {
                Player match = new Player();

                //set player information
                match.Id = projections.Players.Count + 1;
                match.Name = player.Name;
                match.Position = player.Position;
                match.NflTeam = player.NflTeam;

                //find matches in statistics and week projections
                Player weekProjectionPlayer = weekProjectionPlayers.Where(p => p.NflTeam == player.NflTeam && p.Position == player.Position && p.Name == player.Name).FirstOrDefault<Player>();

                //calculate weights
                int gamesPlayed = player.GamesPlayed;
                int gamesProjected = (weekProjectionPlayer == null) ? 0 : 1;
                int divisor = gamesPlayed + gamesProjected;

                //add stats
                match.PassingYards += player.PassingYards;
                match.PassingTouchdowns += player.PassingTouchdowns;
                match.Interceptions += player.Interceptions;
                match.RushingYards += player.RushingYards;
                match.RushingTouchdowns += player.RushingTouchdowns;
                match.Receptions += player.Receptions;
                match.ReceivingYards += player.ReceivingYards;
                match.ReceivingTouchdowns += player.ReceivingTouchdowns;

                if (weekProjectionPlayer != null)
                {
                    //add stats
                    match.PassingYards += weekProjectionPlayer.PassingYards;
                    match.PassingTouchdowns += weekProjectionPlayer.PassingTouchdowns;
                    match.Interceptions += weekProjectionPlayer.Interceptions;
                    match.RushingYards += weekProjectionPlayer.RushingYards;
                    match.RushingTouchdowns += weekProjectionPlayer.RushingTouchdowns;
                    match.Receptions += weekProjectionPlayer.Receptions;
                    match.ReceivingYards += weekProjectionPlayer.ReceivingYards;
                    match.ReceivingTouchdowns += weekProjectionPlayer.ReceivingTouchdowns;

                    //remove player for faster looping later
                    weekProjectionPlayers.Remove(weekProjectionPlayer);
                }

                int bye = Byes[match.NflTeam];
                int gamesRemaining = 17 - CurrentWeek + ((bye >= CurrentWeek) ? 0 : 1);

                //turn combination of stats into ROS projections
                match.PassingYards = match.PassingYards * gamesRemaining / divisor;
                match.PassingTouchdowns = match.PassingTouchdowns * gamesRemaining / divisor;
                match.Interceptions = match.Interceptions * gamesRemaining / divisor;
                match.RushingYards = match.RushingYards * gamesRemaining / divisor;
                match.RushingTouchdowns = match.RushingTouchdowns * gamesRemaining / divisor;
                match.Receptions = match.Receptions * gamesRemaining / divisor;
                match.ReceivingYards = match.ReceivingYards * gamesRemaining / divisor;
                match.ReceivingTouchdowns = match.ReceivingTouchdowns * gamesRemaining / divisor;

                //add match to players
                projections.Players.Add(match);
            }
        }

        private List<string> GetAlternateTeam(string team)
        {
            switch (team)
            {
                case "ARZ": return new List<string> { "ARI" };
                case "BLT": return new List<string> { "BAL" };
                case "CLV": return new List<string> { "CLE" };
                case "GB": return new List<string> { "GBP" };
                case "HST": return new List<string> { "HOU" };
                case "JAX": return new List<string> { "JAC" };
                case "KC": return new List<string> { "KCC" };
                case "LA": return new List<string> { "RAM", "STL" };
                case "NE": return new List<string> { "NEP" };
                case "NO": return new List<string> { "NOS" };
                case "SD": return new List<string> { "SDC" };
                case "SF": return new List<string> { "SFO" };
                case "TB": return new List<string> { "TBB" };
                case "WAS": return new List<string> { "WSH" };
                default: return new List<string> { team };
            }
        }

        private List<string> GetAlternateNames(string name)
        {
            switch (name)
            {
                case "Adrian L. Peterson": return new List<string> { "Adrian Peterson" };
                case "Ben Watson": return new List<string> { "Benjamin Watson" };
                case "David A. Johnson": return new List<string> { "David Johnson" };
                case "Duke Johnson": return new List<string> { "Duke Johnson Jr." };
                case "Jonathan C. Stewart": return new List<string> { "Jonathan Stewart" };
                case "Odell Beckham Jr.": return new List<string> { "Odell Beckham" };
                case "Robert Griffin III": return new List<string> { "Robert Griffin" };
                case "Steve L. Smith": return new List<string> { "Steve Smith Sr.", "Steve Smith" };
                case "Ted Ginn": return new List<string> { "Ted Ginn Jr." };
                default: return new List<string> { name };
            }
        }

        private int GetCurrentWeek()
        {
            DateTime today = DateTime.Today;
            DateTime endWeekOne = new DateTime(2016, 9, 12);
            DateTime endWeekTwo = new DateTime(2016, 9, 19);
            DateTime endWeekThree = new DateTime(2016, 9, 26);
            DateTime endWeekFour = new DateTime(2016, 10, 3);
            DateTime endWeekFive = new DateTime(2016, 10, 10);
            DateTime endWeekSix = new DateTime(2016, 10, 17);
            DateTime endWeekSeven = new DateTime(2016, 10, 24);
            DateTime endWeekEight = new DateTime(2016, 10, 31);
            DateTime endWeekNine = new DateTime(2016, 11, 7);
            DateTime endWeekTen = new DateTime(2016, 11, 14);
            DateTime endWeekEleven = new DateTime(2016, 11, 21);
            DateTime endWeekTwelve = new DateTime(2016, 11, 28);
            DateTime endWeekThirteen = new DateTime(2016, 12, 5);
            DateTime endWeekFourteen = new DateTime(2016, 12, 12);
            DateTime endWeekFifteen = new DateTime(2016, 12, 19);
            DateTime endWeekSixteen = new DateTime(2016, 12, 26);
            DateTime endWeekSeventeen = new DateTime(2017, 1, 1);

            if (today <= endWeekOne) { return 1; } //week 1
            else if (endWeekOne <= endWeekTwo) { return 2; } //week 2
            else if (endWeekOne <= endWeekTwo) { return 3; } //week 3
            else if (endWeekOne <= endWeekTwo) { return 4; } //week 4
            else if (endWeekOne <= endWeekTwo) { return 5; } //week 5
            else if (endWeekOne <= endWeekTwo) { return 6; } //week 6
            else if (endWeekOne <= endWeekTwo) { return 7; } //week 7
            else if (endWeekOne <= endWeekTwo) { return 8; } //week 8
            else if (endWeekOne <= endWeekTwo) { return 9; } //week 9
            else if (endWeekOne <= endWeekTwo) { return 10; } //week 10
            else if (endWeekOne <= endWeekTwo) { return 11; } //week 11
            else if (endWeekOne <= endWeekTwo) { return 12; } //week 12
            else if (endWeekOne <= endWeekTwo) { return 13; } //week 13
            else if (endWeekOne <= endWeekTwo) { return 14; } //week 14
            else if (endWeekOne <= endWeekTwo) { return 15; } //week 15
            else if (endWeekOne <= endWeekTwo) { return 16; } //week 16
            else if (endWeekOne <= endWeekTwo) { return 17; } //week 17
            else { return 0; } //after season
        }

        private string IsNullStat(object o)
        {
            if (o == null) { return "0"; }
            else { return o.ToString(); }
        }
    }
}
