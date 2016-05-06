using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.HostParsers
{
    public class MFLParser : IHostParser
    {
        private const string LoginUrl = "www60.myfantasyleague.com";
        private const string LeagueTableId = "standings";
        private const string TeamLinkClass = "franchise_";
        private const string TeamTableId = "single_roster";

        public MFLParser() { }

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
                        row.Name == "tr" &&
                        row.Attributes["class"] != null
            ).ToList<HtmlNode>();

            //loop through rows and get anchors with url and team name
            foreach (HtmlNode row in rows)
            {
                HtmlNode anchor = row.Descendants().Where(a => a.Name == "a" &&
                                                        a.Attributes["class"] != null &&
                                                        a.Attributes["class"].Value.Contains(TeamLinkClass))
                .FirstOrDefault<HtmlNode>();

                Team team = new Team();
                team.Id = leagueData.Teams.Count + 1;
                team.Name = anchor.InnerHtml.Replace("&#39;", "'");
                team.Url = anchor.Attributes["href"].Value.Replace("O=01", "O=07");

                leagueData.Teams.Add(team);
            }
        }

        public void ParseTeam(HtmlDocument document, League league, Team team, Projections projections)
        {
            //get table containing players of teams
            HtmlNode teamTable = document.GetElementbyId(TeamTableId).Descendants().Where(t => t.Name == "table").FirstOrDefault<HtmlNode>();

            //get all rows with id's because they are the rows that have players
            List<HtmlNode> rows = teamTable.Descendants().Where(row => row.Name == "tr" && 
                                                                row.Attributes.Count > 0 && 
                                                                row.Attributes["class"] != null && 
                                                                row.Attributes["class"].Value.ToString() != "").ToList<HtmlNode>();

            foreach (HtmlNode row in rows)
            {
                HtmlNode playerCell = row.SelectSingleNode("./td[1]");

                if (playerCell != null && playerCell.Attributes["class"] != null && playerCell.Attributes["class"].Value == "player")
                {
                    string playerTeamPosition = playerCell.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>().InnerText;

                    //find indecies of start of team and position
                    int positionStart = playerTeamPosition.LastIndexOf(" ") + 1;
                    int teamStart = playerTeamPosition.Substring(0, positionStart - 1).LastIndexOf(" ");

                    //split player names into array to rearange them
                    string[] playerNames = playerTeamPosition.Substring(0, teamStart).Split(',');

                    //get player attributes
                    string playerName = playerNames[1].Trim() + " " + playerNames[0].Trim();
                    string playerPosition = playerTeamPosition.Substring(positionStart, playerTeamPosition.Length - positionStart).Trim();
                    string playerTeam = playerTeamPosition.Substring(teamStart, positionStart - teamStart - 1).Trim();

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