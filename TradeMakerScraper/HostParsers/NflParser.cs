using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.HostParsers
{
    public class NflParser : IHostParser
    {
        private const string LoginUrl = "https://registerdisney.go.com/jgc/v2/client/ESPN-FANTASYLM-PROD/guest/login?langPref=en-US";
        //private const string LoginUrl = "https://espn.go.com/login/";
        private const string LeagueTableClass = "tableType-team";
        private const string TeamLinkClass = "team";
        private const string TeamTableClass = "tableType-player";
        private const string PlayerRowClass = "player";
        private const string PlayerCellClass = "playerNameAndInfo";

        public NflParser() { }

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
            //HtmlNodeCollection leagueTables = document.DocumentNode.SelectNodes("//table[contains(@class,'" + LeagueTableClass + "')]");
            List<HtmlNode> leagueTables = document.DocumentNode.Descendants().Where(t => t.Name == "table" && t.Attributes["class"] != null && t.Attributes["class"].Value.Contains(LeagueTableClass)).Skip(1).ToList<HtmlNode>();

            List<HtmlNode> rows = new List<HtmlNode>();

            //get all the rows that contain the team links
            foreach (HtmlNode leagueTable in leagueTables)
            {
                rows.AddRange(
                    leagueTable.Descendants().Where(
                        row => row.Attributes.Count > 0 &&
                        row.Name == "tr" &&
                        row.Attributes["class"] != null &&
                        row.Attributes["class"].Value.Contains(TeamLinkClass)
                    ).ToList<HtmlNode>()
                );
            }

            //loop through rows and get anchors with url and team name
            foreach (HtmlNode row in rows)
            {
                //HtmlNode anchor = row.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>();
                HtmlNode anchor = row.Descendants().Where(a => a.Name == "a").Skip(1).FirstOrDefault<HtmlNode>();
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
            //HtmlNode teamTable = document.DocumentNode.SelectNodes("//table[contains(@class,'" + LeagueTableClass + "')]").FirstOrDefault<HtmlNode>();
            HtmlNode teamTable = document.DocumentNode.Descendants().Where(t => t.Name == "table" && t.Attributes["class"] != null && t.Attributes["class"].Value.Contains(TeamTableClass)).FirstOrDefault<HtmlNode>(); // 

            //get all rows with id's because they are the rows that have players
            HtmlNode body = teamTable.Descendants().Where(b => b.Name == "tbody").FirstOrDefault<HtmlNode>();
            List<HtmlNode> rows = body.Descendants().Where(row => row.Name == "tr" && row.Attributes["class"] != null && row.Attributes["class"].Value.Contains(PlayerRowClass)).ToList<HtmlNode>();

            foreach (HtmlNode row in rows)
            {
                //HtmlNode playerCell = row.SelectSingleNode("./td[4]").FirstChild;
                //List<HtmlNode> nodes = row.Descendants().Where(c => c.Name == "td" && row.Attributes["class"] != null && row.Attributes["class"].Value.Contains(PlayerCellClass)).ToList<HtmlNode>();
                HtmlNode playerCell = row.Descendants().Where(c => c.Name == "td" && c.Attributes["class"] != null && c.Attributes["class"].Value == PlayerCellClass).FirstOrDefault();

                if (playerCell != null && playerCell.Id != null)
                {
                    HtmlNode playerNameAnchor = playerCell.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>();

                    if (playerNameAnchor != null)
                    {
                        //get player attributes
                        string playerName = playerCell.Descendants().Where(a => a.Name == "a").FirstOrDefault<HtmlNode>().InnerText;

                        string[] positionTeam = playerCell.Descendants().Where(a => a.Name == "em").FirstOrDefault<HtmlNode>().InnerText.Split('-');
                        string playerPosition = positionTeam[0].Trim();
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
    }
}