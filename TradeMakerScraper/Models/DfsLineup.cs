using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class DfsLineup
    {
        public PlayerList Quarterbacks { get; set; }
        public PlayerList RunningBacks { get; set; }
        public PlayerList WideReceivers { get; set; }
        public PlayerList TightEnds { get; set; }
        public PlayerList Kickers { get; set; }
        public PlayerList Defenses { get; set; }
        public int Salary { get; set; }
        public decimal FantasyPoints { get; set; }

        public void CalculateSalary()
        {

        }
    }
}