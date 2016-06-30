using HtmlAgilityPack;
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

        // GET api/projectionscraper
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Projections Get()
        {
            Projections projections = new Projections();

            //get full season projections
            GetSeasonQbProjections(ref projections);
            GetSeasonRbProjections(ref projections);
            GetSeasonWrProjections(ref projections);
            GetSeasonTeProjections(ref projections);

            //get season statistics
            GetSeasonQbStatistics(ref projections);

            //get next week projections
            GetNextWeekQbProjections(ref projections);
            GetNextWeekRbProjections(ref projections);
            GetNextWeekWrProjections(ref projections);
            GetNextWeekTeProjections(ref projections);

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

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = parser.Player;
                player.AlternateNames = GetAlternateNames(parser.Player);
                player.Position = "QB";
                player.NflTeam = parser.Team;
                player.NflAlternateTeams = GetAlternateTeam(parser.Team);
                player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);

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

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = parser.Player;
                player.AlternateNames = GetAlternateNames(parser.Player);
                player.Position = "RB";
                player.NflTeam = parser.Team;
                player.NflAlternateTeams = GetAlternateTeam(parser.Team);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);

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

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = parser.Player;
                player.AlternateNames = GetAlternateNames(parser.Player);
                player.Position = "WR";
                player.NflTeam = parser.Team;
                player.NflAlternateTeams = GetAlternateTeam(parser.Team);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);

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

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = parser.Player;
                player.AlternateNames = GetAlternateNames(parser.Player);
                player.Position = "TE";
                player.NflTeam = parser.Team;
                player.NflAlternateTeams = GetAlternateTeam(parser.Team);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[2]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);

                //add datarow to datatable
                projections.SeasonProjectionPlayers.Add(player);
            }
        }

        private void GetSeasonQbStatistics(ref Projections projections)
        {
            
        }

        private void GetNextWeekQbProjections(ref Projections projections)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/qb.php");

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
                    player.Id = projections.Players.Count + 1;
                    player.Name = parser.Player;
                    player.AlternateNames = GetAlternateNames(parser.Player);
                    player.Position = "QB";
                    player.NflTeam = parser.Team;
                    player.NflAlternateTeams = GetAlternateTeam(parser.Team);
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
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/rb.php");

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
                    player.Id = projections.Players.Count + 1;
                    player.Name = parser.Player;
                    player.AlternateNames = GetAlternateNames(parser.Player);
                    player.Position = "RB";
                    player.NflTeam = parser.Team;
                    player.NflAlternateTeams = GetAlternateTeam(parser.Team);
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
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/wr.php");

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
                    player.Id = projections.Players.Count + 1;
                    player.Name = parser.Player;
                    player.AlternateNames = GetAlternateNames(parser.Player);
                    player.Position = "WR";
                    player.NflTeam = parser.Team;
                    player.NflAlternateTeams = GetAlternateTeam(parser.Team);
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
            HtmlDocument document = scraper.Scrape("https://www.fantasypros.com/nfl/projections/te.php");

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
                    player.Id = projections.Players.Count + 1;
                    player.Name = parser.Player;
                    player.AlternateNames = GetAlternateNames(parser.Player);
                    player.Position = "TE";
                    player.NflTeam = parser.Team;
                    player.NflAlternateTeams = GetAlternateTeam(parser.Team);
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[2]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);

                    //add datarow to datatable
                    projections.WeekProjectionPlayers.Add(player);
                }
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
    }
}
