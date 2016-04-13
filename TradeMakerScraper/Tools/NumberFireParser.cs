using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Tools
{
    public class NumberFireParser
    {
        public string Player { get; set; }
        public string Position { get; set; }
        public string Team { get; set; }
        public string AlternateTeam { get; set; }

        public NumberFireParser(string player)
        {
            Player = GetNameFromNumberFirePlayer(player);
            Position = GetPositionFromNumberFirePlayer(player);
            Team = GetTeamFromNumberFirePlayer(player);
            AlternateTeam = ConvertTeamToAlternateTeam(Team);
        }

        public string GetNameFromNumberFirePlayer(string playerData)
        {
            int firstParenIndex = playerData.IndexOf("(");
            return playerData.Substring(0, firstParenIndex).Replace("\r", "").Replace("\n", "").Trim(' ');
        }

        public string GetPositionFromNumberFirePlayer(string playerData)
        {
            int firstParenIndex = playerData.IndexOf("(");
            int commaIndex = playerData.IndexOf(',');
            int length = commaIndex - firstParenIndex - 1;
            return playerData.Substring(firstParenIndex + 1, length).Trim(' ');
        }

        public string GetTeamFromNumberFirePlayer(string playerData)
        {
            int commaIndex = playerData.IndexOf(',');
            int lastParenIndex = playerData.IndexOf(')');
            int length = lastParenIndex - commaIndex - 1;

            return playerData.Substring(commaIndex + 1, length).Trim(' ');
        }

        public string ConvertTeamToAlternateTeam(string team)
        {
            switch (team)
            {
                case "WSH":
                    return "WAS";
                default:
                    return team;
            }
        }
    }
}