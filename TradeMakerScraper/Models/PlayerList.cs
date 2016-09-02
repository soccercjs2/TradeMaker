using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class PlayerList : IEquatable<PlayerList>
    {
        public HashSet<Player> Players { get; set; }
        public int Salary { get; set; }
        public decimal FantasyPoints { get; set; }
        public decimal CostPerPoint { get; set; }

        public PlayerList()
        {
            Players = new HashSet<Player>();
        }

        public bool Equals(PlayerList other)
        {
            bool equal = true;

            if (other.Players.Count == Players.Count)
            {
                foreach (Player otherPlayer in other.Players)
                {
                    bool foundPlayer = false;

                    foreach (Player player in Players)
                    {
                        if (player == otherPlayer) { foundPlayer = true; }
                    }

                    if (!foundPlayer) { equal = false; }
                }
            }
            else
            {
                return false;
            }

            return equal;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (Player player in Players)
            {
                hash += player.Id * (int)player.FantasyPoints * player.Name.Length;
            }

            return hash;
        }
    }
}