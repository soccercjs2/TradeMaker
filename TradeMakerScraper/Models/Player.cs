using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NflTeam { get; set; }
        public string NflAlternateTeam { get; set; }
        public string Position { get; set; }
        public decimal PassingYards { get; set; }
        public decimal PassingTouchdowns { get; set; }
        public decimal Interceptions { get; set; }
        public decimal RushingYards { get; set; }
        public decimal RushingTouchdowns { get; set; }
        public decimal Receptions { get; set; }
        public decimal ReceivingYards { get; set; }
        public decimal ReceivingTouchdowns { get; set; }
        public decimal FantasyPoints { get; set; }
        public bool Required { get; set; }
        public bool Excluded { get; set; }
        public bool NewPlayer { get; set; }
        public bool OldPlayer { get; set; }
        public decimal TradeValue { get; set; }

        public Player()
        {
            Required = false;
            Excluded = false;
            NewPlayer = false;
            OldPlayer = false;
        }
    }
}