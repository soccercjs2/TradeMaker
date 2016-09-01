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
        private const string FantasyProsDataTable = "data-table";
        private const string FanDuelUrl = "https://www.fantasypros.com/nfl/fanduel-cheatsheet.php";
        private const string DraftKingsUrl = "https://www.fantasypros.com/nfl/draftkings-cheatsheet.php";
        private const string FantasyAcesUrl = "https://www.fantasypros.com/nfl/fantasyaces-cheatsheet.php";
        private const string DraftpotUrl = "https://www.fantasypros.com/nfl/draftpot-cheatsheet.php";
        private const string FantasyDraftUrl = "https://www.fantasypros.com/nfl/fantasydraft-cheatsheet.php";
        private const string YahooUrl = "https://www.fantasypros.com/nfl/yahoo-cheatsheet.php";

        private const string QbSuffix = "?position=QB";
        private const string RbSuffix = "?position=RB";
        private const string WrSuffix = "?position=WR";
        private const string TeSuffix = "?position=TE";
        private const string KSuffix = "?position=K";
        private const string DSTSuffix = "?position=DST";

        private const string OldFanDuelUrl = "https://web.archive.org/web/20150909224821/http://www.fantasypros.com/nfl/fanduel-cheatsheet.php";
        private const string OldDraftKingsUrl = "https://web.archive.org/web/20150913113405/http://www.fantasypros.com/nfl/draftkings-cheatsheet.php";
        private const string OldFantasyAcesUrl = "https://web.archive.org/web/20150905061618/http://www.fantasypros.com/nfl/fantasyaces-cheatsheet.php";
        private const string OldDraftpotUrl = "https://web.archive.org/web/20150905064620/http://www.fantasypros.com/nfl/draftpot-cheatsheet.php";
        //private const string OldFantasyDraftUrl = "https://www.fantasypros.com/nfl/fantasydraft-cheatsheet.php";
        private const string OldYahooUrl = "https://web.archive.org/web/20150905170413/http://www.fantasypros.com/nfl/yahoo-cheatsheet.php";

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public DfsData Post(DFSScraperPackage package)
        {
            DfsData dfsData = new DfsData();

            //if (dfsSite == "FanDuel") { GetNextWeekData(ref dfsData, FanDuelUrl); }
            //else if (dfsSite == "DraftKings") { GetNextWeekData(ref dfsData, DraftKingsUrl); }
            //else if (dfsSite == "FantasyAces") { GetNextWeekData(ref dfsData, FantasyAcesUrl); }
            //else if (dfsSite == "Draftpot") { GetNextWeekData(ref dfsData, DraftpotUrl); }
            //else if (dfsSite == "FantasyDraft") { GetNextWeekData(ref dfsData, FantasyDraftUrl); }
            //else if (dfsSite == "Yahoo") { GetNextWeekData(ref dfsData, YahooUrl); }

            if (package.DFSSite == "FanDuel") { GetNextWeekData(ref dfsData, OldFanDuelUrl); }
            else if (package.DFSSite == "DraftKings") { GetNextWeekData(ref dfsData, OldDraftKingsUrl); }
            else if (package.DFSSite == "FantasyAces") { GetNextWeekData(ref dfsData, OldFantasyAcesUrl); }
            else if (package.DFSSite == "Draftpot") { GetNextWeekData(ref dfsData, OldDraftpotUrl); }
            //else if (dfsSite == "FantasyDraft") { GetNextWeekData(ref dfsData, OldFantasyDraftUrl); }
            else if (package.DFSSite == "Yahoo") { GetNextWeekData(ref dfsData, OldYahooUrl); }

            dfsData.Quarterbacks.OrderByDescending(p => p.FantasyPoints);
            dfsData.RunningBacks.OrderByDescending(p => p.FantasyPoints);
            dfsData.WideReceivers.OrderByDescending(p => p.FantasyPoints);
            dfsData.TightEnds.OrderByDescending(p => p.FantasyPoints);

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
            HtmlDocument document = scraper.Scrape(url + QbSuffix);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //if player has cost per point for this week
                    if (row.SelectSingleNode("./td[13]").InnerText.Length > 0)
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
                        player.FantasyPoints = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText.Replace("pts", ""));
                        player.Salary = int.Parse(row.SelectSingleNode("./td[12]").InnerText.Replace("$", "").Replace(",", ""));
                        player.CostPerPoint = int.Parse(row.SelectSingleNode("./td[13]").InnerText.Replace("$", ""));

                        //add datarow to datatable
                        dfsData.Quarterbacks.Add(player);
                    }
                }
            }
        }

        private void GetNextWeekRbData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url + RbSuffix);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //if player has cost per point for this week
                    if (row.SelectSingleNode("./td[13]").InnerText.Length > 0)
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
                        player.FantasyPoints = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText.Replace("pts", ""));
                        player.Salary = int.Parse(row.SelectSingleNode("./td[12]").InnerText.Replace("$", "").Replace(",", ""));
                        player.CostPerPoint = int.Parse(row.SelectSingleNode("./td[13]").InnerText.Replace("$", ""));

                        //add datarow to datatable
                        dfsData.RunningBacks.Add(player);
                    }
                }
            }
        }

        private void GetNextWeekWrData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url + WrSuffix);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //if player has cost per point for this week
                    if (row.SelectSingleNode("./td[13]").InnerText.Length > 0)
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
                        player.FantasyPoints = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText.Replace("pts", ""));
                        player.Salary = int.Parse(row.SelectSingleNode("./td[12]").InnerText.Replace("$", "").Replace(",", ""));
                        player.CostPerPoint = int.Parse(row.SelectSingleNode("./td[13]").InnerText.Replace("$", ""));

                        //add datarow to datatable
                        dfsData.WideReceivers.Add(player);
                    }
                }
            }
        }

        private void GetNextWeekTeData(ref DfsData dfsData, string url)
        {
            WebScraper scraper = new WebScraper(null, null, null);
            HtmlDocument document = scraper.Scrape(url + TeSuffix);

            //get projection-data table from html
            HtmlNode table = document.GetElementbyId(FantasyProsDataTable).Descendants().Where(t => t.Name == "tbody").FirstOrDefault<HtmlNode>();

            //loop through rows in projection table
            //loop through rows in projection table
            List<HtmlNode> rows = table.SelectNodes("./tr").ToList<HtmlNode>();

            if (rows.Count > 1)
            {
                foreach (HtmlNode row in rows)
                {
                    //if player has cost per point for this week
                    if (row.SelectSingleNode("./td[13]").InnerText.Length > 0)
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
                        player.FantasyPoints = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText.Replace("pts", ""));
                        player.Salary = int.Parse(row.SelectSingleNode("./td[12]").InnerText.Replace("$", "").Replace(",", ""));
                        player.CostPerPoint = int.Parse(row.SelectSingleNode("./td[13]").InnerText.Replace("$", ""));

                        //add datarow to datatable
                        dfsData.TightEnds.Add(player);
                    }
                }
            }
        }
    }
}
