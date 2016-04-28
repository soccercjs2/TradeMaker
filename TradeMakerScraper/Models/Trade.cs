using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Models
{
    public class Trade : IEquatable<Trade>
    {
        public string MyTeamName { get; set; }
        public string TheirTeamName { get; set; }
        public IEnumerable<Player> MyPlayers { get; set; }
        public IEnumerable<Player> TheirPlayers { get; set; }
        public Roster MyNewStartingRoster { get; set; }
        public Roster MyOldStartingRoster { get; set; }
        public Roster TheirNewStartingRoster { get; set; }
        public Roster TheirOldStartingRoster { get; set; }
        public decimal MyDifferential { get; set; }
        public decimal TheirDifferential { get; set; }
        public decimal CompositeDifferential { get; set; }
        public decimal Fairness { get; set; }

        public Trade()
        {

        }

        public Trade(string myTeamName, string theirTeamName, IEnumerable<Player> myPlayers, IEnumerable<Player> theirPlayers)
        {
            MyTeamName = myTeamName.ToUpper();
            TheirTeamName = theirTeamName.ToUpper();
            MyPlayers = myPlayers.OrderByDescending(p => p.TradeValue);
            TheirPlayers = theirPlayers.OrderByDescending(p => p.TradeValue);
            Fairness = theirPlayers.Sum(p => p.TradeValue) - myPlayers.Sum(p => p.TradeValue);
        }

        public void CalculateDifferentials(LeagueData leagueData, TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool)
        {
            //get new original rosters
            MyOldStartingRoster = myTeamPlayerPool.OptimalLineUp(leagueData, MyPlayers);
            TheirOldStartingRoster = theirTeamPlayerPool.OptimalLineUp(leagueData, TheirPlayers);

            //get new starting rosters
            MyNewStartingRoster = myTeamPlayerPool.OptimalLineUp(leagueData, TheirPlayers, MyPlayers);
            TheirNewStartingRoster = theirTeamPlayerPool.OptimalLineUp(leagueData, MyPlayers, TheirPlayers);

            //calculate differentials
            MyDifferential = MyNewStartingRoster.Points - MyOldStartingRoster.Points;
            TheirDifferential = TheirNewStartingRoster.Points - TheirOldStartingRoster.Points;
            CompositeDifferential = MyDifferential + TheirDifferential;

            //set differentials back in rosters
            MyNewStartingRoster.Differential = "+" + MyDifferential;
            MyOldStartingRoster.Differential = "-" + MyDifferential;
            TheirNewStartingRoster.Differential = "+" + TheirDifferential;
            TheirOldStartingRoster.Differential = "-" + TheirDifferential;
        }

        public override int GetHashCode()
        {
            int hashCode = (int)(
                Math.Pow((double)MyPlayers.Count(), 2) + Math.Pow((double)TheirPlayers.Count(), 2) +
                Math.Pow((double)MyDifferential, 2) + Math.Pow((double)TheirDifferential, 2) +
                Math.Pow((double)CompositeDifferential, 2) + Math.Pow((double)Fairness, 2) +
                Math.Pow((double)MyPlayers.Sum(p => p.FantasyPoints), 2) + Math.Pow((double)TheirPlayers.Sum(p => p.FantasyPoints), 2)
            );

            return hashCode;
        }

        public bool Equals(Trade other)
        {
            if (MyPlayers.Count() != other.MyPlayers.Count()) { return false; }
            if (TheirPlayers.Count() != other.TheirPlayers.Count()) { return false; }

            List<int> myPlayerIds = new List<int>();
            List<int> myOtherPlayerIds = new List<int>();
            foreach (Player player in MyPlayers) { myPlayerIds.Add(player.Id); }
            foreach (Player player in other.MyPlayers) { myOtherPlayerIds.Add(player.Id); }

            for (int i = 0; i < myPlayerIds.Count(); i++)
            {
                if (myPlayerIds[i] != myOtherPlayerIds[i]) { return false; }
            }

            List<int> theirPlayerIds = new List<int>();
            List<int> theirOtherPlayerIds = new List<int>();
            foreach (Player player in TheirPlayers) { theirPlayerIds.Add(player.Id); }
            foreach (Player player in other.TheirPlayers) { theirOtherPlayerIds.Add(player.Id); }

            for (int i = 0; i < theirPlayerIds.Count(); i++)
            {
                if (theirPlayerIds[i] != theirOtherPlayerIds[i]) { return false; }
            }

            return true;
        }
    }
}