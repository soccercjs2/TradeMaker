using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.HostParsers
{
    public class YahooParser : IHostParser
    {
        private const string LoginUrl = "https://www.yahoo.com";
        private const string LeagueTableId = "standingstable";
        private const string TeamLinkClass = "Linkable";
        private const string TeamTableId = "statTable0";
        private const string PlayerClass = "ysf-player-name";

        public YahooParser() { }

        public string GetLoginUrl()
        {
            return LoginUrl;
        }

        public string GetPostData(string username, string password)
        {
            //return string.Format("""loginValue"":""soccercjs2@gmail.com"",""password"":""Unitednumber2.""", username, password);
            return "{ \"loginValue\":\"soccercjs2@gmail.com\",\"password\":\"Unitednumber2.\"}";
        }

        public void ParseLeague(HtmlDocument document, ref LeagueData leagueData)
        {
            //get table containing team names and team urls
            HtmlNode leagueTable = document.GetElementbyId(LeagueTableId);

            //get all the rows that contain the team links
            List<HtmlNode> rows = leagueTable.Descendants().Where(
                        row => row.Attributes.Count > 0 &&
                        row.Name == "tr" &&
                        row.Attributes["class"] != null &&
                        row.Attributes["class"].Value.Contains(TeamLinkClass)
            ).ToList<HtmlNode>();

            //loop through rows and get anchors with url and team name
            foreach (HtmlNode row in rows)
            {
                List<HtmlNode> anchors = row.Descendants().Where(a => a.Name == "a").ToList<HtmlNode>();
                HtmlNode anchor = anchors[1];

                Team team = new Team();
                team.Id = leagueData.Teams.Count + 1;
                team.Name = anchor.InnerHtml.Replace("&#39;", "'");
                team.Url = "https://football.fantasysports.yahoo.com" + anchor.Attributes["href"].Value;

                leagueData.Teams.Add(team);
            }
        }

        public void ParseTeam(HtmlDocument document, League league, Team team, Projections projections)
        {
            //get table containing players of teams
            HtmlNode teamTable = document.GetElementbyId(TeamTableId);

            //get all rows with id's because they are the rows that have players
            List<HtmlNode> playerContainers = teamTable.Descendants().Where(row => row.Name == "div" && 
                                                                row.Attributes["class"] != null &&
                                                                row.Attributes["class"].Value.Contains(PlayerClass)
            ).ToList<HtmlNode>();

            foreach (HtmlNode playerContainer in playerContainers)
            {
                if (playerContainer != null && playerContainer.Id != null)
                {
                    HtmlNode nameAnchor = playerContainer.Descendants().Where(a => a.Name == "a" &&
                                                                              a.Attributes["class"] != null &&
                                                                              a.Attributes["class"].Value.Contains("name")
                    ).FirstOrDefault<HtmlNode>();

                    //if nameanchor is null, then roster slot is empty
                    if (nameAnchor != null)
                    {
                        HtmlNode teamPositionSpan = playerContainer.Descendants().Where(a => a.Name == "span").FirstOrDefault<HtmlNode>();

                        //find indecies of start of team and position
                        int positionStart = teamPositionSpan.InnerText.LastIndexOf(" ") + 1;
                        int teamLength = teamPositionSpan.InnerText.IndexOf(" ");

                        //get player attributes
                        string playerName = nameAnchor.InnerText;
                        string playerPosition = teamPositionSpan.InnerText.Substring(positionStart);
                        string playerTeam = teamPositionSpan.InnerText.Substring(0, teamLength).ToUpper();

                        Player player = projections.Players.Where(
                            p => p.Name == playerName &&
                                p.Position == playerPosition &&
                                (p.NflTeam == playerTeam || p.NflAlternateTeam == playerTeam)
                        ).FirstOrDefault<Player>();

                        if (player != null)
                        {
                            projections.Players.Remove(player);
                            team.Players.Add(player);
                        }
                    }
                }
            }
        }
    }
}