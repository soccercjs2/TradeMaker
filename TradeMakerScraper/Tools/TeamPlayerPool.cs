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
        public HashSet<PlayerList> OnePlayerTradePool { get; set; }
        public HashSet<PlayerList> TwoPlayerTradePool { get; set; }
        public HashSet<PlayerList> ThreePlayerTradePool { get; set; }

        public TeamPlayerPool(Team team)
        {
            Team = team;
            TradablePlayers = team.Players.Where(p => p.TradeValue > 0).ToList();
            OnePlayerTradePool = GetOnePlayerTradePool();
            TwoPlayerTradePool = GetTwoPlayerTradePool();
            ThreePlayerTradePool = GetThreePlayerTradePool();
        }

        public Roster OptimalLineUp(LeagueData leagueData, HashSet<Player> lostPlayers)
        {
            List<Player> team = new List<Player>(Team.Players);
            IEnumerable<Player> lost = new List<Player>(lostPlayers);
            OptimalRosterMaker rosterMaker = new OptimalRosterMaker();
            return rosterMaker.GetRoster(leagueData, team, lost);
        }

        public Roster OptimalLineUp(LeagueData leagueData, HashSet<Player> gainedPlayers, HashSet<Player> lostPlayers)
        {
            List<Player> team = new List<Player>(Team.Players);
            IEnumerable<Player> gained = new List<Player>(gainedPlayers);
            IEnumerable<Player> lost = new List<Player>(lostPlayers);

            OptimalRosterMaker rosterMaker = new OptimalRosterMaker();
            return rosterMaker.GetRoster(leagueData, team, gained, lost);
        }

        private HashSet<PlayerList> GetOnePlayerTradePool()
        {
            IEnumerable<PlayerList> foundTradeSides =   from firstPlayer in TradablePlayers
                                                        select new PlayerList() { Players = { firstPlayer } };
            //select new HashSet<Player>() { firstPlayer };

            HashSet<PlayerList> efficientTradeSides = new HashSet<PlayerList>();

            foreach (PlayerList tradeSide in foundTradeSides)
            {
                efficientTradeSides.Add(tradeSide);
            }

            return efficientTradeSides;
        }

        private HashSet<PlayerList> GetTwoPlayerTradePool()
        {
            IEnumerable<PlayerList> foundTradeSides = from firstPlayer in TradablePlayers
                                                      from secondPlayer in TradablePlayers
                                                      where firstPlayer != secondPlayer
                                                      select new PlayerList() { Players = { firstPlayer, secondPlayer } };

            HashSet<PlayerList> efficientTradeSides = new HashSet<PlayerList>();

            foreach (PlayerList tradeSide in foundTradeSides)
            {
                efficientTradeSides.Add(tradeSide);
            }

            return efficientTradeSides;
        }

        private HashSet<PlayerList> GetThreePlayerTradePool()
        {
            IEnumerable<PlayerList> foundTradeSides = from firstPlayer in TradablePlayers
                                                      from secondPlayer in TradablePlayers
                                                      from thirdPlayer in TradablePlayers
                                                      where firstPlayer != secondPlayer && firstPlayer != thirdPlayer && secondPlayer != thirdPlayer
                                                      select new PlayerList() { Players = { firstPlayer, secondPlayer, thirdPlayer } };

            HashSet<PlayerList> efficientTradeSides = new HashSet<PlayerList>();

            foreach (PlayerList tradeSide in foundTradeSides)
            {
                efficientTradeSides.Add(tradeSide);
            }

            return efficientTradeSides;
        }
    }
}