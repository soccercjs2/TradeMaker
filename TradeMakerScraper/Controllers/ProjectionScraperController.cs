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

        // GET api/projectionscraper
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Projections Get()
        {
            Projections projections = new Projections();

            GetQuarterbacks(ref projections);
            GetRunningBacks(ref projections);
            GetWideReceivers(ref projections);
            GetTightEnds(ref projections);

            return projections;
        }

        private void GetQuarterbacks(ref Projections projections)
        {
            //get qb data
            string html = "";

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String path = Directory.GetCurrentDirectory() + "\\Html\\PffQbProjections.html";
            html = File.ReadAllText(path);

            //create html object and load html into it
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html
            HtmlNode projectionTable = document.GetElementbyId(PffProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = row.SelectSingleNode("./td[2]").InnerText;
                player.AlternateNames = GetAlternateNames(row.SelectSingleNode("./td[2]").InnerText);
                player.Position = "QB";
                player.NflTeam = row.SelectSingleNode("./td[3]").InnerText;
                player.NflAlternateTeams = GetAlternateTeam(row.SelectSingleNode("./td[3]").InnerText);
                player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
                player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[10]").InnerText);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[13]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[14]").InnerText);

                //add datarow to datatable
                projections.Players.Add(player);
            }
        }

        private void GetRunningBacks(ref Projections projections)
        {
            //get rb data
            string html = "";

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String path = Directory.GetCurrentDirectory() + "\\Html\\PffRbProjections.html";
            html = File.ReadAllText(path);

            //create html object and load html into it
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html
            HtmlNode projectionTable = document.GetElementbyId(PffProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = row.SelectSingleNode("./td[2]").InnerText;
                player.AlternateNames = GetAlternateNames(row.SelectSingleNode("./td[2]").InnerText);
                player.Position = "RB";
                player.NflTeam = row.SelectSingleNode("./td[3]").InnerText;
                player.NflAlternateTeams = GetAlternateTeam(row.SelectSingleNode("./td[3]").InnerText);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[10]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[12]").InnerText);

                //add datarow to datatable
                projections.Players.Add(player);
            }
        }

        private void GetWideReceivers(ref Projections projections)
        {
            //get rb data
            string html = "";

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String path = Directory.GetCurrentDirectory() + "\\Html\\PffWrProjections.html";
            html = File.ReadAllText(path);

            //create html object and load html into it
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html
            HtmlNode projectionTable = document.GetElementbyId(PffProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = row.SelectSingleNode("./td[2]").InnerText;
                player.AlternateNames = GetAlternateNames(row.SelectSingleNode("./td[2]").InnerText);
                player.Position = "WR";
                player.NflTeam = row.SelectSingleNode("./td[3]").InnerText;
                player.NflAlternateTeams = GetAlternateTeam(row.SelectSingleNode("./td[3]").InnerText);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[12]").InnerText);

                //add datarow to datatable
                projections.Players.Add(player);
            }
        }

        private void GetTightEnds(ref Projections projections)
        {
            //get rb data
            string html = "";

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String path = Directory.GetCurrentDirectory() + "\\Html\\PffTeProjections.html";
            html = File.ReadAllText(path);

            //create html object and load html into it
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html
            HtmlNode projectionTable = document.GetElementbyId(PffProjectionTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();

                //set row values
                player.Id = projections.Players.Count + 1;
                player.Name = row.SelectSingleNode("./td[2]").InnerText;
                player.AlternateNames = GetAlternateNames(row.SelectSingleNode("./td[2]").InnerText);
                player.Position = "TE";
                player.NflTeam = row.SelectSingleNode("./td[3]").InnerText;
                player.NflAlternateTeams = GetAlternateTeam(row.SelectSingleNode("./td[3]").InnerText);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);

                //add datarow to datatable
                projections.Players.Add(player);
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

        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        //public Projections Get()
        //{
        //    HashSet<Player> players = new HashSet<Player>();

        //    //get qb data
        //    string html = "";
        //    //Thread qbThread = new Thread(() => html = ScrapeBrowser(qbProjectionUrl));
        //    //qbThread.SetApartmentState(ApartmentState.STA);
        //    //qbThread.Start();
        //    //qbThread.Join();

        //    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        //    String path = Directory.GetCurrentDirectory() + "\\Html\\NumberFireProjections.html";
        //    html = File.ReadAllText(path);

        //    //create html object and load html into it
        //    HtmlDocument document = new HtmlDocument();
        //    document.LoadHtml(html);

        //    //get projection-data table from html
        //    HtmlNode projectionTable = document.GetElementbyId(NumberFireProjectionTable);

        //    //loop through rows in projection-data
        //    foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
        //    {
        //        //create new datarow
        //        Player player = new Player();
        //        NumberFireParser numberFireParser = new NumberFireParser(row.SelectSingleNode("./td[1]").InnerText);

        //        //set row values
        //        player.Id = players.Count + 1;
        //        player.Name = numberFireParser.Player;
        //        player.Position = numberFireParser.Position;
        //        player.NflTeam = numberFireParser.Team;
        //        player.NflAlternateTeam = numberFireParser.AlternateTeam;
        //        player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
        //        player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
        //        player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
        //        player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
        //        player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[10]").InnerText);
        //        player.Receptions = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText);
        //        player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[12]").InnerText);
        //        player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[13]").InnerText);
        //        player.FantasyPoints = 0;

        //        //add datarow to datatable
        //        players.Add(player);
        //    }

        //    Projections projections = new Projections();
        //    projections.Players = players;
        //    return projections;
        //}
    }
}
