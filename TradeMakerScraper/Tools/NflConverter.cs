using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class NflConverter
    {
        public string Name { get; set; }
        public string NflTeam { get; set; }

        public NflConverter(string name, string nflTeam)
        {
            Name = GetNflName(name);
            NflTeam = GetNflTeam(nflTeam);
        }

        private string GetNflName(string name)
        {
            switch (name)
            {
                //case "Adrian L. Peterson": return "Adrian Peterson";
                //case "Ben Watson": return "Benjamin Watson";
                //case "David A. Johnson": return "David Johnson";
                //case "Duke Johnson": return "Duke Johnson Jr.";
                //case "Jonathan C. Stewart": return "Jonathan Stewart";
                //case "Odell Beckham Jr.": return "Odell Beckham";
                //case "Robert Griffin III": return "Robert Griffin";
                //case "Ted Ginn": return "Ted Ginn Jr.";
                case "A.J. McCarron": return "AJ McCarron";
                case "DeVante Parker": return "Devante Parker";
                case "Odell Beckham Jr.": return "Odell Beckham";
                case "Ted Ginn Jr.": return "Ted Ginn";
                default: return name;
            }
        }

        private string GetNflTeam(string nflTeam)
        {
            if (nflTeam != null) { nflTeam = nflTeam.ToUpper(); }

            switch (nflTeam)
            {
                //case "ARZ": return "ARI";
                //case "BLT": return "BAL";
                //case "CLV": return "CLE";
                //case "HST": return "HOU";
                case "NEP": return "NE";
                case "TBB": return "TB";
                case "SFO": return "SF";
                case "RAM": return "LA";
                case "KCC": return "KC";
                case "NOS": return "NO";
                case "GBP": return "GB";
                case "JAC": return "JAX";
                case "SDC": return "SD";
                default: return nflTeam;
            }
        }
    }
}