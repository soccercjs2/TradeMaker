using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Models
{
    public class Trade : IEquatable<Trade>
    {
        public IEnumerable<Player> MyPlayers { get; set; }
        public IEnumerable<Player> TheirPlayers { get; set; }
        public IEnumerable<Player> MyStartingRoster { get; set; }
        public IEnumerable<Player> TheirStartingRoster { get; set; }
        public decimal MyDifferential { get; set; }
        public decimal TheirDifferential { get; set; }
        public decimal CompositeDifferential { get; set; }
        public decimal Fairness { get; set; }
        public string MyNameList { get; set; }
        public string TheirNameList { get; set; }

        public Trade()
        {

        }

        public Trade(IEnumerable<Player> myPlayers, IEnumerable<Player> theirPlayers)
        {
            MyPlayers = myPlayers.OrderByDescending(p => p.TradeValue);
            TheirPlayers = theirPlayers.OrderByDescending(p => p.TradeValue);
            Fairness = theirPlayers.Sum(p => p.TradeValue) - myPlayers.Sum(p => p.TradeValue);
            MyNameList = GetPlayersString(MyPlayers);
            TheirNameList = GetPlayersString(TheirPlayers);
        }

        public void CalculateDifferentials(LeagueData leagueData, TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool)
        {
            //get new original rosters
            List<Player> myOriginalStartingRoster = myTeamPlayerPool.OptimalLineUp(leagueData).ToList();
            List<Player> theirOriginalStartingRoster = theirTeamPlayerPool.OptimalLineUp(leagueData).ToList();

            //calculate original starting lineup points
            decimal myOriginalStartingPoints = myOriginalStartingRoster.Sum(p => p.FantasyPoints);
            decimal theirOriginalStartingPoints = theirOriginalStartingRoster.Sum(p => p.FantasyPoints);

            //get new starting rosters
            MyStartingRoster = myTeamPlayerPool.OptimalLineUp(leagueData, TheirPlayers, MyPlayers);
            TheirStartingRoster = theirTeamPlayerPool.OptimalLineUp(leagueData, MyPlayers, TheirPlayers);

            //calculate new starting lineup points
            decimal myNewStartingPoints = MyStartingRoster.Sum(p => p.FantasyPoints);
            decimal theirNewStartingPoints = TheirStartingRoster.Sum(p => p.FantasyPoints);

            //calculate differentials
            MyDifferential = myNewStartingPoints - myOriginalStartingPoints;
            TheirDifferential = theirNewStartingPoints - theirOriginalStartingPoints;
            CompositeDifferential = MyDifferential + TheirDifferential;
        }

        public string GetPlayersString(IEnumerable<Player> players)
        {
            //initialize string to return
            string playersString = "";

            //loop through players to build string
            foreach (Player player in players)
            {
                playersString += "<div>" + player.Name + "(" + player.TradeValue + ")</div>";
            }

            //return result
            return playersString;
        }

        public override int GetHashCode()
        {
            int hashCode = (int)(
                Math.Pow((double)MyPlayers.Count(), 2) + Math.Pow((double)TheirPlayers.Count(), 2) +
                Math.Pow((double)MyDifferential, 2) + Math.Pow((double)TheirDifferential, 2) +
                Math.Pow((double)CompositeDifferential, 2) + Math.Pow((double)Fairness, 2) +
                Math.Pow((double)MyPlayers.Sum(p => p.FantasyPoints), 2) + Math.Pow((double)TheirPlayers.Sum(p => p.FantasyPoints), 2) +
                Math.Pow((double)MyNameList.Length, 2) + Math.Pow((double)TheirNameList.Length, 2)
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

        //public TradeView GetTradeView()
        //{
        //    TradeView tradeView = new TradeView();

        //    tradeView.MyPlayersHtml = MyNameList;
        //    tradeView.TheirPlayersHtml = TheirNameList;
        //    tradeView.MyDifferential = MyDifferential;
        //    tradeView.TheirDifferential = TheirDifferential;
        //    tradeView.Fairness = Fairness;

        //    return tradeView;
        //}
    }
}