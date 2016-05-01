using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class FantasyPointsCalculator
    {
        public decimal FantasyPoints { get; set; }
        public FantasyPointsCalculator(League league, Player player)
        {
            FantasyPoints = player.PassingYards / league.YardsPerFantasyPoint +
                            player.PassingTouchdowns * league.PointsPerPassingTouchdown -
                            player.Interceptions * league.PointsLostPerInterception +
                            player.RushingYards / 10 +
                            player.RushingTouchdowns * 6 +
                            player.Receptions * league.PointsPerReception +
                            player.ReceivingYards / 10 +
                            player.ReceivingTouchdowns * 6;
        }
    }
}