using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class LeagueData
    {
        public List<Team> Teams { get; set; }
        public Team Waivers { get; set; }
    }
}