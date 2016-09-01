using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class DfsLineup
    {
        public List<IEnumerable> Quarterbacks { get; set; }
        public List<IEnumerable> RunningBacks { get; set; }
        public List<IEnumerable> WideReceivers { get; set; }
        public List<IEnumerable> TightEnds { get; set; }
        public List<IEnumerable> Kickers { get; set; }
        public List<IEnumerable> Defenses { get; set; }
    }
}