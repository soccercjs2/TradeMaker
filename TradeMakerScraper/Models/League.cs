using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class League
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool RequiresLogin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}