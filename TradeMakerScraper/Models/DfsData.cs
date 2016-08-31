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
    }
}