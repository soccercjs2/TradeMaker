using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TradeMakerScraper.HostParsers;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Controllers
{
    public class DFSScraperController : ApiController
    {
        private const string FantasyProsDataTable = "data";
        private const string FanDuelUrl = "https://www.fantasypros.com/nfl/fanduel-cheatsheet.php";
        private const string DraftKingsUrl = "https://www.fantasypros.com/nfl/draftkings-cheatsheet.php";
        private const string FantasyAcesUrl = "https://www.fantasypros.com/nfl/fantasyaces-cheatsheet.php";
        private const string DraftpotUrl = "https://www.fantasypros.com/nfl/draftpot-cheatsheet.php";
        private const string FantasyDraftUrl = "https://www.fantasypros.com/nfl/fantasydraft-cheatsheet.php";
        private const string YahooUrl = "https://www.fantasypros.com/nfl/yahoo-cheatsheet.php";

        private const string OldFanDuelUrl = "https://web.archive.org/web/20150909224821/http://www.fantasypros.com/nfl/fanduel-cheatsheet.php";
        private const string OldDraftKingsUrl = "https://web.archive.org/web/20150913113405/http://www.fantasypros.com/nfl/draftkings-cheatsheet.php";
        private const string OldFantasyAcesUrl = "https://web.archive.org/web/20150905061618/http://www.fantasypros.com/nfl/fantasyaces-cheatsheet.php";
        private const string OldDraftpotUrl = "https://web.archive.org/web/20150905064620/http://www.fantasypros.com/nfl/draftpot-cheatsheet.php";
        //private const string OldFantasyDraftUrl = "https://www.fantasypros.com/nfl/fantasydraft-cheatsheet.php";
        private const string OldYahooUrl = "https://web.archive.org/web/20150905170413/http://www.fantasypros.com/nfl/yahoo-cheatsheet.php";

        private int CurrentWeek;

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public DfsData Get(string dfsSite)
        {
            DfsData dfsData = new DfsData();

            //CurrentWeek = GetCurrentWeek();

            //if (dfsSite == "FanDuel") { GetNextWeekData(ref dfsData, FanDuelUrl); }
            //else if (dfsSite == "DraftKings") { GetNextWeekData(ref dfsData, DraftKingsUrl); }
            //else if (dfsSite == "FantasyAces") { GetNextWeekData(ref dfsData, FantasyAcesUrl); }
            //else if (dfsSite == "Draftpot") { GetNextWeekData(ref dfsData, DraftpotUrl); }
            //else if (dfsSite == "FantasyDraft") { GetNextWeekData(ref dfsData, FantasyDraftUrl); }
            //else if (dfsSite == "Yahoo") { GetNextWeekData(ref dfsData, YahooUrl); }

            if (dfsSite == "FanDuel") { GetNextWeekData(ref dfsData, OldFanDuelUrl); }
            else if (dfsSite == "DraftKings") { GetNextWeekData(ref dfsData, OldDraftKingsUrl); }
            else if (dfsSite == "FantasyAces") { GetNextWeekData(ref dfsData, OldFantasyAcesUrl); }
            else if (dfsSite == "Draftpot") { GetNextWeekData(ref dfsData, OldDraftpotUrl); }
            //else if (dfsSite == "FantasyDraft") { GetNextWeekData(ref dfsData, OldFantasyDraftUrl); }
            else if (dfsSite == "Yahoo") { GetNextWeekData(ref dfsData, OldYahooUrl); }

            return dfsData;
        }

        private void GetNextWeekData(ref DfsData dfsData, string url)
        {
            GetNextWeekQbData(ref dfsData, url);
            GetNextWeekRbData(ref dfsData, url);
            GetNextWeekWrData(ref dfsData, url);
            GetNextWeekTeData(ref dfsData, url);
        }

        private void GetNextWeekQbData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

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
                    player.Id = dfsData.Quarterbacks.Count + 1;
                    player.Name = parser.Name;
                    player.Position = "QB";
                    player.NflTeam = parser.Team;
                    player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                    player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                    player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                    player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[8]").InnerText);
                    player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);

                    //add datarow to datatable
                    dfsData.WeekProjectionPlayers.Add(player);
                }
            }
        }

        private void GetNextWeekRbData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

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
                    player.Id = dfsData.RunningBacks.Count + 1;
                    player.Name = parser.Name;
                    player.Position = "RB";
                    player.NflTeam = parser.Team;
                    player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);

                    //add datarow to datatable
                    dfsData.RunningBacks.Add(player);
                }
            }
        }

        private void GetNextWeekWrData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

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
                    player.Id = dfsData.WideReceivers.Count + 1;
                    player.Name = parser.Name;
                    player.Position = "WR";
                    player.NflTeam = parser.Team;
                    player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);

                    //add datarow to datatable
                    dfsData.WideReceivers.Add(player);
                }
            }
        }

        private void GetNextWeekTeData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

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
                    player.Id = dfsData.TightEnds.Count + 1;
                    player.Name = parser.Name;
                    player.Position = "TE";
                    player.NflTeam = parser.Team;
                    player.Receptions = decimal.Parse(row.SelectSingleNode("./td[2]").InnerText);
                    player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[3]").InnerText);
                    player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[4]").InnerText);

                    //add datarow to datatable
                    dfsData.TightEnds.Add(player);
                }
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
    }
}
