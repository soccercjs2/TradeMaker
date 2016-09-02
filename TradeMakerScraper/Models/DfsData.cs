using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class DfsData
    {
        public HashSet<Player> Quarterbacks { get; set; }
        public HashSet<Player> RunningBacks { get; set; }
        public HashSet<Player> WideReceivers { get; set; }
        public HashSet<Player> TightEnds { get; set; }
        public HashSet<Player> Kickers { get; set; }
        public HashSet<Player> Defenses { get; set; }

        public DfsData()
        {
            Quarterbacks = new HashSet<Player>();
            RunningBacks = new HashSet<Player>();
            WideReceivers = new HashSet<Player>();
            TightEnds = new HashSet<Player>();
            Kickers = new HashSet<Player>();
            Defenses = new HashSet<Player>();
        }
    }
}