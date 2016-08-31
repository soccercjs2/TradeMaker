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
        public HashSet<Player> MyPlayers { get; set; }
        public HashSet<Player> TheirPlayers { get; set; }
        public Roster MyNewStartingRoster { get; set; }
        public Roster MyOldStartingRoster { get; set; }
        public Roster TheirNewStartingRoster { get; set; }
        public Roster TheirOldStartingRoster { get; set; }
        public decimal MyDifferential { get; set; }
        public string MyGains { get; set; }
        public string MyLosses { get; set; }
        public string TheirGains { get; set; }
        public string TheirLosses { get; set; }
        public decimal TheirDifferential { get; set; }
        public decimal CompositeDifferential { get; set; }
        public decimal Fairness { get; set; }
        public bool HasLamePlayers { get; set; }

        public Trade()
        {

        }

        public Trade(string myTeamName, string theirTeamName, PlayerList myPlayers, PlayerList theirPlayers)
        {
            MyTeamName = myTeamName.ToUpper();
            TheirTeamName = theirTeamName.ToUpper();
            //MyPlayers = myPlayers.OrderByDescending(p => p.TradeValue);
            //TheirPlayers = theirPlayers.OrderByDescending(p => p.TradeValue);
            MyPlayers = myPlayers.Players;
            TheirPlayers = theirPlayers.Players;
            Fairness = theirPlayers.Players.Sum(p => p.TradeValue) - myPlayers.Players.Sum(p => p.TradeValue);
        }

        public bool CalculateDifferentials(LeagueData leagueData, TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool)
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

            //my positional differentials
            decimal myQbDifferential = MyNewStartingRoster.QbPoints - MyOldStartingRoster.QbPoints;
            decimal myRbDifferential = MyNewStartingRoster.RbPoints - MyOldStartingRoster.RbPoints;
            decimal myWrDifferential = MyNewStartingRoster.WrPoints - MyOldStartingRoster.WrPoints;
            decimal myTeDifferential = MyNewStartingRoster.TePoints - MyOldStartingRoster.TePoints;
            decimal myFlexDifferential = MyNewStartingRoster.FlexPoints - MyOldStartingRoster.FlexPoints;

            //their positional differentials
            decimal theirQbDifferential = TheirNewStartingRoster.QbPoints - TheirOldStartingRoster.QbPoints;
            decimal theirRbDifferential = TheirNewStartingRoster.RbPoints - TheirOldStartingRoster.RbPoints;
            decimal theirWrDifferential = TheirNewStartingRoster.WrPoints - TheirOldStartingRoster.WrPoints;
            decimal theirTeDifferential = TheirNewStartingRoster.TePoints - TheirOldStartingRoster.TePoints;
            decimal theirFlexDifferential = TheirNewStartingRoster.FlexPoints - TheirOldStartingRoster.FlexPoints;

            foreach (Player player in TheirPlayers)
            {
                if (player.Position == "QB" && myQbDifferential < 10) { return false; }
                else if (player.Position == "RB" && myRbDifferential > 0 && myRbDifferential < 10) { return false; }
                else if (player.Position == "WR" && myWrDifferential > 0 && myWrDifferential < 10) { return false; }
                else if (player.Position == "TE" && myTeDifferential < 10) { return false; }
            }

            foreach (Player player in MyPlayers)
            {
                if (player.Position == "QB" && theirQbDifferential > 0 && theirQbDifferential < 10) { return false; }
                else if (player.Position == "RB" && theirRbDifferential > 0 && theirRbDifferential < 10) { return false; }
                else if (player.Position == "WR" && theirWrDifferential > 0 && theirWrDifferential < 10) { return false; }
                else if (player.Position == "TE" && theirTeDifferential > 0 && theirTeDifferential < 10) { return false; }
            }

            //calculate my positional differentials and add them to the proper change string
            AddMyChange("QB", MyNewStartingRoster.QbPoints - MyOldStartingRoster.QbPoints);
            AddMyChange("RB", MyNewStartingRoster.RbPoints - MyOldStartingRoster.RbPoints);
            AddMyChange("WR", MyNewStartingRoster.WrPoints - MyOldStartingRoster.WrPoints);
            AddMyChange("TE", MyNewStartingRoster.TePoints - MyOldStartingRoster.TePoints);
            AddMyChange("FLEX", MyNewStartingRoster.FlexPoints - MyOldStartingRoster.FlexPoints);

            //calculate their positional differentials and add them to the proper change string
            AddTheirChange("QB", TheirNewStartingRoster.QbPoints - TheirOldStartingRoster.QbPoints);
            AddTheirChange("RB", TheirNewStartingRoster.RbPoints - TheirOldStartingRoster.RbPoints);
            AddTheirChange("WR", TheirNewStartingRoster.WrPoints - TheirOldStartingRoster.WrPoints);
            AddTheirChange("TE", TheirNewStartingRoster.TePoints - TheirOldStartingRoster.TePoints);
            AddTheirChange("FLEX", TheirNewStartingRoster.FlexPoints - TheirOldStartingRoster.FlexPoints);

            //set differentials back in rosters
            MyNewStartingRoster.Differential = "+" + MyDifferential;
            MyOldStartingRoster.Differential = "-" + MyDifferential;
            TheirNewStartingRoster.Differential = "+" + TheirDifferential;
            TheirOldStartingRoster.Differential = "-" + TheirDifferential;

            return true;
        }

        private void AddMyChange(string position, decimal differential)
        {
            if (differential > 0)
            {
                if (MyGains == null) { MyGains = "[" + position + " +" + differential + "]"; }
                else { MyGains += "[" + position + " +" + differential + "]"; }
            }
            else if (differential < 0)
            {
                if (MyLosses == null) { MyLosses = "[" + position + " " + differential + "]"; }
                else { MyLosses += "[" + position + " " + differential + "]"; }
            }
        }

        private void AddTheirChange(string position, decimal differential)
        {
            string change = "[" + position + " " + differential + "]";

            if (differential > 0)
            {
                if (TheirGains == null) { TheirGains = "[" + position + " +" + differential + "]"; }
                else { TheirGains += "[" + position + " +" + differential + "]"; }
            }
            else if (differential < 0)
            {
                if (TheirLosses == null) { TheirLosses = "[" + position + " " + differential + "]"; }
                else { TheirLosses += "[" + position + " " + differential + "]"; }
            }
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