using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class Roster
    {
        public List<Player> Quarterbacks { get; set; }
        public List<Player> RunningBacks { get; set; }
        public List<Player> WideReceivers { get; set; }
        public List<Player> TightEnds { get; set; }
        public List<Player> Flexes { get; set; }

        public Roster()
        {
            Quarterbacks = new List<Player>();
            RunningBacks = new List<Player>();
            WideReceivers = new List<Player>();
            TightEnds = new List<Player>();
            Flexes = new List<Player>();
        }

        public decimal GetPoints()
        {
            return
                Quarterbacks.Sum(p => p.FantasyPoints) +
                RunningBacks.Sum(p => p.FantasyPoints) +
                WideReceivers.Sum(p => p.FantasyPoints) +
                TightEnds.Sum(p => p.FantasyPoints) +
                Flexes.Sum(p => p.FantasyPoints);
        }
    }
}