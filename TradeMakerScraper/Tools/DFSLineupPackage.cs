using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class DFSLineupPackage
    {
        public IEnumerable<Player> Quarterbacks { get; set; }
        public IEnumerable<Player> RunningBacks { get; set; }
        public IEnumerable<Player> WideReceivers { get; set; }
        public IEnumerable<Player> TightEnds { get; set; }
        public IEnumerable<Player> Kickers { get; set; }
        public IEnumerable<Player> Defenses { get; set; }
        public int SalaryCap { get; set; }
    }
}