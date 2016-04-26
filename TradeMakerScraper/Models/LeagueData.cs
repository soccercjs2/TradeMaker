using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class LeagueData
    {
        public List<Team> Teams { get; set; }
        public Player WaiverQuarterback { get; set; }
        public Player WaiverRunningBack { get; set; }
        public Player WaiverWideReceiver { get; set; }
        public Player WaiverTightEnd { get; set; }
        public Team MyTeam { get; set; }
        public Team TheirTeam { get; set; }
        public bool UseAllTeams { get; set; }
        public List<Trade> Trades { get; set; }

        public LeagueData()
        {
            Teams = new List<Team>();
            Trades = new List<Trade>();
        }
    }
}