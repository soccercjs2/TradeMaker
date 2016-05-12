using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.HostParsers
{
    public class EspnParser : IHostParser
    {
        private const string LoginUrl = "https://registerdisney.go.com/jgc/v2/client/ESPN-FANTASYLM-PROD/guest/login?langPref=en-US";
        //private const string LoginUrl = "https://espn.go.com/login/";
        private const string LeagueTableId = "xstandTbl_div";
        private const string TeamLinkClass = "sortableRow";
        private const string TeamTableId = "playertable_0";

        public EspnParser() { }

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
            HtmlNodeCollection leagueTables = document.DocumentNode.SelectNodes("//table[contains(@id,'" + LeagueTableId + "')]");

            List<HtmlNode> rows = new List<HtmlNode>();

            //get all the rows that contain the team links
            foreach (HtmlNode leagueTable in leagueTables)
            {
                rows.AddRange(
                    leagueTable.Descendants().Where(
                        row => row.Attributes.Count > 0 &&
                        row.Attributes["class"] != null &&
                        row.Attributes["class"].Value.Contains(TeamLinkClass)
                    ).ToList<HtmlNode>()
                );
            }

            //loop through rows and get anchors with url and team name
            foreach (HtmlNode row in rows)
            {
                HtmlNode anchor = row.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>();
                Team team = new Team();
                team.Id = leagueData.Teams.Count + 1;
                team.Name = anchor.InnerHtml.Replace("&#39;", "'");
                team.Url = "http://games.espn.go.com" + anchor.Attributes["href"].Value.Replace("&amp;", "&");

                leagueData.Teams.Add(team);
            }
        }

        public void ParseTeam(HtmlDocument document, League league, Team team, Projections projections)
        {
            //get table containing players of teams
            HtmlNode teamTable = document.GetElementbyId(TeamTableId);

            //get all rows with id's because they are the rows that have players
            List<HtmlNode> rows = teamTable.Descendants().Where(row => row.Name == "tr" && row.Id != null && row.Id != "").ToList<HtmlNode>();

            foreach (HtmlNode row in rows)
            {
                HtmlNode playerCell = row.SelectSingleNode("./td[2]");

                if (playerCell != null && playerCell.Id != null)
                {
                    HtmlNode statusNode = playerCell.Descendants().Where(s => s.Name == "span").FirstOrDefault<HtmlNode>();
                    if (statusNode != null) { playerCell.RemoveChild(statusNode); }

                    string playerTeamPosition = playerCell.InnerText.Replace("&nbsp;", " ").Trim();

                    //find indecies of start of team and position
                    int positionStart = playerTeamPosition.LastIndexOf(" ") + 1;
                    int teamStart = playerTeamPosition.Substring(0, positionStart - 1).LastIndexOf(" ");

                    //get player attributes
                    string playerName = playerTeamPosition.Substring(0, teamStart - 1);
                    string playerPosition = playerTeamPosition.Substring(positionStart, playerTeamPosition.Length - positionStart).Trim();
                    string playerTeam = playerTeamPosition.Substring(teamStart, positionStart - teamStart - 1).Trim().ToUpper();

                    Player player = projections.Players.Where(
                        p => (p.Name == playerName || p.AlternateNames.Contains(playerName)) &&
                            p.Position == playerPosition &&
                            (p.NflTeam == playerTeam || p.NflAlternateTeams.Contains(playerTeam))
                    ).FirstOrDefault<Player>();

                    if (player != null)
                    {
                        projections.Players.Remove(player);
                        team.Players.Add(player);
                    }
                    else if (playerPosition == "QB" || playerPosition == "RB" || playerPosition == "WR" || playerPosition == "TE")
                    {
                        projections.UnMatchedPlayers.Add(playerName + ";" + playerPosition + ";" + playerTeam);
                    }
                }
            }
        }
    }
}