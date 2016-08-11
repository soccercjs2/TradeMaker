using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.HostParsers
{
    public class CbsSportsParser : IHostParser
    {
        private const string LoginUrl = "https://registerdisney.go.com/jgc/v2/client/ESPN-FANTASYLM-PROD/guest/login?langPref=en-US";
        //private const string LoginUrl = "https://espn.go.com/login/";
        private const string LeagueTableClass = "standings";
        private const string TeamTableClass = "data";
        private const string PlayerRowClass = "playerRow";
        private const string PlayerNameAnchor = "playerLink";
        private const string PlayerPositionAndTeamClass = "playerPositionAndTeam";

        public CbsSportsParser() { }

        public string GetLoginUrl()
        {
            return LoginUrl;
        }

        public string GetPostData(string username, string password)
        {
            return "";
        }

        public void ParseLeague(HtmlDocument document, ref LeagueData leagueData)
        {
            //get table containing team names and team urls
            List<HtmlNode> leagueTables = document.DocumentNode.Descendants().Where(t => t.Name == "table" && t.Attributes["class"] != null && t.Attributes["class"].Value.Contains(LeagueTableClass)).ToList<HtmlNode>();

            List<HtmlNode> rows = new List<HtmlNode>();

            //get all the rows that contain the team links
            foreach (HtmlNode leagueTable in leagueTables)
            {
                rows.AddRange(
                    leagueTable.Descendants().Where(
                        row => row.Attributes.Count > 0 &&
                        row.Name == "tr" &&
                        row.Attributes["id"] != null
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
                team.Url = "http://fantasy.nfl.com" + anchor.Attributes["href"].Value.Replace("&amp;", "&");

                leagueData.Teams.Add(team);
            }
        }

        public void ParseTeam(HtmlDocument document, League league, Team team, Projections projections)
        {
            //get table containing players of teams
            HtmlNode teamTable = document.DocumentNode.Descendants().Where(t => t.Name == "table" && t.Attributes["class"] != null && t.Attributes["class"].Value.Contains(TeamTableClass)).FirstOrDefault<HtmlNode>(); // 

            //get all rows with id's because they are the rows that have players
            HtmlNode body = teamTable.Descendants().Where(b => b.Name == "tbody").FirstOrDefault<HtmlNode>();
            List<HtmlNode> rows = body.Descendants().Where(row => row.Name == "tr" && row.Attributes["class"] != null && row.Attributes["class"].Value.Contains(PlayerRowClass)).ToList<HtmlNode>();

            foreach (HtmlNode row in rows)
            {
                //get player attributes
                string playerName = row.Descendants().Where(a => a.Name == "a" && row.Attributes["class"] != null && row.Attributes["class"].Value.Contains(PlayerNameAnchor)).FirstOrDefault<HtmlNode>().InnerText;
                string[] positionTeam = row.Descendants().Where(s => s.Name == "span" && row.Attributes["class"] != null && row.Attributes["class"].Value.Contains(PlayerPositionAndTeamClass)).FirstOrDefault<HtmlNode>().InnerText.Split('|');
                string playerPosition = positionTeam[0];
                string playerTeam = (positionTeam.Length > 1) ? positionTeam[1].Trim() : null;

                //convert name and team to nfl values
                NflConverter converter = new NflConverter(playerName, playerTeam);

                //find player
                Player player = projections.Players.Where(
                    p => p.Name == converter.Name && p.Position == playerPosition && p.NflTeam == converter.NflTeam
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