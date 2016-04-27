using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class Roster
    {
        public List<RosterPlayer> Quarterbacks { get; set; }
        public List<RosterPlayer> RunningBacks { get; set; }
        public List<RosterPlayer> WideReceivers { get; set; }
        public List<RosterPlayer> TightEnds { get; set; }
        public List<RosterPlayer> Flexes { get; set; }

        public Roster()
        {
            Quarterbacks = new List<RosterPlayer>();
            RunningBacks = new List<RosterPlayer>();
            WideReceivers = new List<RosterPlayer>();
            TightEnds = new List<RosterPlayer>();
            Flexes = new List<RosterPlayer>();
        }

        public decimal GetPoints()
        {
            return
                Quarterbacks.Sum(p => p.Player.FantasyPoints) +
                RunningBacks.Sum(p => p.Player.FantasyPoints) +
                WideReceivers.Sum(p => p.Player.FantasyPoints) +
                TightEnds.Sum(p => p.Player.FantasyPoints) +
                Flexes.Sum(p => p.Player.FantasyPoints);
        }
    }
}