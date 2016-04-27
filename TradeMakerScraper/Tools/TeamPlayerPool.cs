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

        public Roster OptimalLineUp(LeagueData leagueData, IEnumerable<Player> lostPlayers)
        {
            List<Player> team = new List<Player>(Team.Players);
            IEnumerable<Player> lost = new List<Player>(lostPlayers);
            OptimalRosterMaker rosterMaker = new OptimalRosterMaker();
            return rosterMaker.GetRoster(leagueData, team, lost);
        }

        public Roster OptimalLineUp(LeagueData leagueData, IEnumerable<Player> gainedPlayers, IEnumerable<Player> lostPlayers)
        {
            List<Player> team = new List<Player>(Team.Players);
            IEnumerable<Player> gained = new List<Player>(gainedPlayers);
            IEnumerable<Player> lost = new List<Player>(lostPlayers);

            OptimalRosterMaker rosterMaker = new OptimalRosterMaker();
            return rosterMaker.GetRoster(leagueData, team, gained, lost);
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