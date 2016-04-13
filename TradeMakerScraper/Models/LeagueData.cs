using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class LeagueData
    {
        public List<Team> Teams { get; set; }
        public List<Player> Waivers { get; set; }

        public LeagueData()
        {
            Teams = new List<Team>();
            Waivers = new List<Player>();
        }
    }
}