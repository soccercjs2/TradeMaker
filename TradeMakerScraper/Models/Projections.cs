using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class Projections
    {
        public HashSet<Player> Players { get; set; }
        public HashSet<Player> SeasonProjectionPlayers { get; set; }
        public HashSet<Player> WeekProjectionPlayers { get; set; }
        public HashSet<Player> StatisticsPlayers { get; set; }
        public HashSet<string> UnMatchedPlayers { get; set; }

        public Projections() {
            Players = new HashSet<Player>();
            SeasonProjectionPlayers = new HashSet<Player>();
            WeekProjectionPlayers = new HashSet<Player>();
            StatisticsPlayers = new HashSet<Player>();
            UnMatchedPlayers = new HashSet<string>();
        }
    }
}