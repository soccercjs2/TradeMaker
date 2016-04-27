using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class RosterPlayer
    {
        public Player Player { get; set; }
        public bool NewPlayer { get; set; }
        public bool OldPlayer { get; set; }

        public RosterPlayer(Player player)
        {
            Player = player;
        }
    }
}