using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class TeamPlayerPool
    {
        public Team Team { get; set; }
        public List<Player> TradablePlayers { get; set; }
        public IEnumerable<IEnumerable<Player>> OnePlayerTradePool { get; set; }
        public IEnumerable<IEnumerable<Player>> TwoPlayerTradePool { get; set; }
        public IEnumerable<IEnumerable<Player>> ThreePlayerTradePool { get; set; }

        public TeamPlayerPool(Team team)
        {
            Team = team;
            TradablePlayers = team.Players.Where(p => p.TradeValue > 0).ToList();
            OnePlayerTradePool = GetOnePlayerTradePool();
            TwoPlayerTradePool = GetTwoPlayerTradePool();
            ThreePlayerTradePool = GetThreePlayerTradePool();
        }

        //public IEnumerable<Player> OptimalLineUp(LeagueData leagueData)
        public Roster OptimalLineUp(LeagueData leagueData)
        {
            return OptimalLineUp(leagueData, null, null);
        }

        //public IEnumerable<Player> OptimalLineUp(LeagueData leagueData, IEnumerable<Player> gainedPlayers, IEnumerable<Player> lostPlayers)
        public Roster OptimalLineUp(LeagueData leagueData, IEnumerable<Player> gainedPlayers, IEnumerable<Player> lostPlayers)
        {
            List<Player> team = new List<Player>(Team.Players);
            if (lostPlayers != null) { foreach (Player lostPlayer in lostPlayers) { team.Remove(lostPlayer); } }
            if (gainedPlayers != null) { foreach (Player gainedPlayer in gainedPlayers) { team.Add(gainedPlayer); } }

            //create list of starters to return
            List<Player> starters = new List<Player>();
            Roster roster = new Roster();

            //get position players
            List<Player> quarterbacks = team.Where(p => p.Position == "QB").OrderByDescending(p => p.FantasyPoints).ToList();
            List<Player> runningBacks = team.Where(p => p.Position == "RB").OrderByDescending(p => p.FantasyPoints).ToList();
            List<Player> wideReceivers = team.Where(p => p.Position == "WR").OrderByDescending(p => p.FantasyPoints).ToList();
            List<Player> tightEnds = team.Where(p => p.Position == "TE").OrderByDescending(p => p.FantasyPoints).ToList();

            //get starting players
            //List<Player> startingQbs = quarterbacks.Take(1).ToList();
            //List<Player> startingRbs = runningBacks.Take(2).ToList();
            //List<Player> startingWrs = wideReceivers.Take(2).ToList();
            //List<Player> startingTes = tightEnds.Take(1).ToList();

            roster.Quarterbacks = quarterbacks.Take(1).ToList();
            roster.RunningBacks = runningBacks.Take(2).ToList();
            roster.WideReceivers = wideReceivers.Take(2).ToList();
            roster.TightEnds = tightEnds.Take(1).ToList();

            //add best waiver to team if missing starter
            //if (startingQbs.Count() < 1) { startingQbs.Add(leagueData.WaiverQuarterback); }
            //if (startingRbs.Count() < 2) { startingRbs.Add(leagueData.WaiverRunningBack); }
            //if (startingWrs.Count() < 2) { startingWrs.Add(leagueData.WaiverWideReceiver); }
            //if (startingTes.Count() < 1) { startingTes.Add(leagueData.WaiverTightEnd); }

            //add best waiver to team if missing starter
            if (roster.Quarterbacks.Count() < 1) { roster.Quarterbacks.Add(leagueData.WaiverQuarterback); }
            if (roster.RunningBacks.Count() < 2) { roster.RunningBacks.Add(leagueData.WaiverRunningBack); }
            if (roster.WideReceivers.Count() < 2) { roster.WideReceivers.Add(leagueData.WaiverWideReceiver); }
            if (roster.TightEnds.Count() < 1) { roster.TightEnds.Add(leagueData.WaiverTightEnd); }

            //get possible waiver players
            Player flexRb = runningBacks.Skip(2).Take(1).FirstOrDefault();
            Player flexWr = wideReceivers.Skip(2).Take(1).FirstOrDefault();
            Player flexTe = tightEnds.Skip(1).Take(1).FirstOrDefault();

            //get waiver points for easy comparing
            decimal flexRbPoints = (flexRb != null) ? flexRb.FantasyPoints : 0;
            decimal flexWrPoints = (flexWr != null) ? flexWr.FantasyPoints : 0;
            decimal flexTePoints = (flexTe != null) ? flexTe.FantasyPoints : 0;

            //add best waiver player to starters and remove him from the bench
            Player flexPlayer;
            if (flexRbPoints > flexWrPoints && flexRbPoints > flexTePoints) { flexPlayer = flexRb; }
            else if (flexWrPoints > flexRbPoints && flexWrPoints > flexTePoints) { flexPlayer = flexWr; }
            else { flexPlayer = flexTe; }

            //foreach (Player player in startingQbs) { starters.Add(player); }
            //foreach (Player player in startingRbs) { starters.Add(player); }
            //foreach (Player player in startingWrs) { starters.Add(player); }
            //foreach (Player player in startingTes) { starters.Add(player); }
            //starters.Add(flexPlayer);

            roster.Flexes.Add(flexPlayer);
            return roster;

            //return optimal starting lineup
            //return starters;
        }

        private IEnumerable<IEnumerable<Player>> GetOnePlayerTradePool()
        {
            return from firstPlayer in TradablePlayers
                   select new List<Player>() { firstPlayer };
        }

        private IEnumerable<IEnumerable<Player>> GetTwoPlayerTradePool()
        {
            return from firstPlayer in TradablePlayers
                   from secondPlayer in TradablePlayers
                   where firstPlayer != secondPlayer
                   select new List<Player>() { firstPlayer, secondPlayer };
        }

        private IEnumerable<IEnumerable<Player>> GetThreePlayerTradePool()
        {
            return from firstPlayer in TradablePlayers
                   from secondPlayer in TradablePlayers
                   from thirdPlayer in TradablePlayers
                   where firstPlayer != secondPlayer && firstPlayer != thirdPlayer && secondPlayer != thirdPlayer
                   select new List<Player>() { firstPlayer, secondPlayer, thirdPlayer };
        }
    }
}