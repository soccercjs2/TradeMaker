using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class FantasyProConverter
    {
        public string Name { get; set; }
        public string NflTeam { get; set; }

        public FantasyProConverter(string name, string nflTeam)
        {
            Name = GetFantasyProName(name.ToUpper());
            NflTeam = GetFantasyProNflTeam(nflTeam.ToUpper());
        }

        private string GetFantasyProName(string name)
        {
            switch (name)
            {
                case "Adrian L. Peterson": return "Adrian Peterson";
                case "Ben Watson": return "Benjamin Watson";
                case "David A. Johnson": return "David Johnson";
                case "Duke Johnson": return "Duke Johnson Jr.";
                case "Jonathan C. Stewart": return "Jonathan Stewart";
                case "Odell Beckham Jr.": return "Odell Beckham";
                case "Robert Griffin III": return "Robert Griffin";
                case "Ted Ginn": return "Ted Ginn Jr.";
                default: return name;
            }
        }

        private string GetFantasyProNflTeam(string nflTeam)
        {
            switch (nflTeam)
            {
                case "ARZ": return "ARI";
                case "BLT": return "BAL";
                case "CLV": return "CLE";
                case "HST": return "HOU";
                case "JAX": return "JAC";
                default: return nflTeam;
            }
        }
    }
}