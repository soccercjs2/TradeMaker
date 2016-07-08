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
                case "A.J. McCarron": return "AJ McCarron";
                case "DeVante Parker": return "Devante Parker";
                case "Duke Johnson Jr.": return "Duke Johnson";
                case "Odell Beckham Jr.": return "Odell Beckham";
                case "Steve Smith Sr.": return "Steve Smith";
                case "Ted Ginn Jr.": return "Ted Ginn";
                default: return name;
            }
        }

        private string GetNflTeam(string nflTeam)
        {
            if (nflTeam != null) { nflTeam = nflTeam.ToUpper(); }

            switch (nflTeam)
            {
                case "NEP": return "NE";
                case "TBB": return "TB";
                case "SFO": return "SF";
                case "RAM": return "LA";
                case "KCC": return "KC";
                case "NOS": return "NO";
                case "GBP": return "GB";
                case "JAC": return "JAX";
                case "SDC": return "SD";
                case "WSH": return "WAS";
                default: return nflTeam;
            }
        }
    }
}