using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Tools
{
    public class FantasyProsParser
    {
        public string Name { get; set; }
        public string Team { get; set; }

        public FantasyProsParser(HtmlNode player)
        {
            Name = GetNameFromFantasyProPlayer(player);
            Team = GetTeamFromFantasyProPlayer(player, Name);
        }

        public string GetNameFromFantasyProPlayer(HtmlNode playerData)
        {
            List<HtmlNode> nodes = playerData.Descendants().ToList<HtmlNode>();
            HtmlNode anchor = playerData.Descendants().Where(t => t.Name == "a").FirstOrDefault<HtmlNode>();
            return anchor.InnerText;
        }

        public string GetTeamFromFantasyProPlayer(HtmlNode playerData, string name)
        {
            List<HtmlNode> nodes = playerData.Descendants().ToList<HtmlNode>();
            HtmlNode team = playerData.Descendants().Where(t => t.Name == "#text" && t.InnerText != name).FirstOrDefault<HtmlNode>();

            if (team == null) { return null; }
            else { return team.InnerText.Trim(); }
        }
    }
}