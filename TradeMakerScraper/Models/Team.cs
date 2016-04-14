using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }

        public Team(int id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
            Players = new List<Player>();
        }
    }
}