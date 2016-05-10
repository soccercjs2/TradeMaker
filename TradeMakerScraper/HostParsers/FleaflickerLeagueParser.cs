using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.HostParsers
{
    public class FleaflickerLeagueParser : IHostParser
    {
        private const string LoginUrl = "http://www.fleaflicker.com/nfl/login";
        private const string LeagueTableId = "table_0";
        private const string TeamLinkClass = "league-name";

        public FleaflickerLeagueParser() { }

        public string GetLoginUrl()
        {
            return LoginUrl;
        }

        public string GetPostData(string username, string password)
        {
            return string.Format(null, username, password);
        }

        public void ParseLeague(HtmlDocument document, ref LeagueData leagueData)
        {
            //get table containing team names and team urls
            HtmlNode leagueTable = document.GetElementbyId(LeagueTableId);

            //get all the rows that contain the team links
            List<HtmlNode> rows = leagueTable.Descendants().Where(
                        row => row.Attributes.Count > 0 &&
                        row.Name == "div" &&
                        row.Attributes["class"] != null &&
                        row.Attributes["class"].Value.Contains(TeamLinkClass)
            ).ToList<HtmlNode>();

            //loop through rows and get anchors with url and team name
            foreach (HtmlNode row in rows)
            {
                HtmlNode anchor = row.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>();
                Team team = new Team();
                team.Id = leagueData.Teams.Count + 1;
                team.Name = anchor.InnerHtml.Replace("&#39;", "'");
                team.Url = "http://www.fleaflicker.com" + anchor.Attributes["href"].Value;

                leagueData.Teams.Add(team);
            }
        }

        public void ParseTeam(HtmlDocument document, League league, Team team, Projections projections)
        {
            //get table containing players of teams
            HtmlNode teamTable = document.GetElementbyId(LeagueTableId);

            //get all rows with id's because they are the rows that have players
            List<HtmlNode> rows = teamTable.Descendants().Where(row => row.Name == "tr" && row.Id != null && row.Id != "").ToList<HtmlNode>();

            foreach (HtmlNode row in rows)
            {
                HtmlNode nameCell = row.SelectSingleNode("./td[1]");
                HtmlNode positionCell = row.SelectSingleNode("./td[2]");
                HtmlNode teamCell = row.SelectSingleNode("./td[3]");

                string playerName = nameCell.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>().InnerText;
                string playerPosition = positionCell.Descendants().Where(s => s.Name == "span").FirstOrDefault<HtmlNode>().InnerText;
                string playerTeam = teamCell.Descendants().Where(s => s.Name == "span").FirstOrDefault<HtmlNode>().InnerText.ToUpper();

                Player player = projections.Players.Where(
                    p => p.Name == playerName &&
                        p.Position == playerPosition &&
                        (p.NflTeam == playerTeam || p.NflAlternateTeam == playerTeam)
                ).FirstOrDefault<Player>();

                if (player != null) {
                    projections.Players.Remove(player);
                    team.Players.Add(player);
                }
            }
        }
    }
}