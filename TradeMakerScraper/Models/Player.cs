using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class Player : IEquatable<Player>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> AlternateNames { get; set; }
        public string NflTeam { get; set; }
        public List<string> NflAlternateTeams { get; set; }
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
        public int GamesPlayed { get; set; }
        public bool Required { get; set; }
        public bool Excluded { get; set; }
        public decimal TradeValue { get; set; }

        public Player()
        {
            Required = false;
            Excluded = false;
        }

        public bool Equals(Player other)
        {
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return (int)(Id + Name.Length + PassingYards + PassingTouchdowns + Interceptions +
                RushingYards + RushingTouchdowns + Receptions + ReceivingYards + ReceivingTouchdowns);
        }
    }
}