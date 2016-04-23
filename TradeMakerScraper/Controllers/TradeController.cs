﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Controllers
{
    public class TradeController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public List<Trade> Post(LeagueData leagueData)
        {
            //create table list to store each trade 
            List<Trade> trades = new List<Trade>();

            //create my team player pool
            TeamPlayerPool myTeamPlayerPool = new TeamPlayerPool(leagueData.MyTeam);

            //create other team list
            List<Team> theirTeams = new List<Team>();
            if (!leagueData.UseAllTeams)
            {
                theirTeams.Add(leagueData.TheirTeam);
            }
            else
            {
                theirTeams = leagueData.Teams;
                theirTeams.Remove(leagueData.Teams.Where(t => t.Id == leagueData.MyTeam.Id).FirstOrDefault<Team>());
            }

            //for each other team, find trades
            foreach (Team theirTeam in theirTeams)
            {
                //load team player pool for this team
                TeamPlayerPool otherTeamPlayerPool = new TeamPlayerPool(theirTeam);

                //find trades with this team
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, otherTeamPlayerPool.OnePlayerTradePool); //1 for 1
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, otherTeamPlayerPool.TwoPlayerTradePool); //1 for 2
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.OnePlayerTradePool, otherTeamPlayerPool.ThreePlayerTradePool); //1 for 3
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, otherTeamPlayerPool.OnePlayerTradePool); //2 for 1
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, otherTeamPlayerPool.TwoPlayerTradePool); //2 for 2
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.TwoPlayerTradePool, otherTeamPlayerPool.ThreePlayerTradePool); //2 for 3
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, otherTeamPlayerPool.OnePlayerTradePool); //3 for 1
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, otherTeamPlayerPool.TwoPlayerTradePool); //3 for 2
                FindTrades(ref trades, leagueData, myTeamPlayerPool, otherTeamPlayerPool, myTeamPlayerPool.ThreePlayerTradePool, otherTeamPlayerPool.ThreePlayerTradePool); //3 for 3
            }

            return trades.OrderByDescending(t => t.CompositeDifferential).Distinct().ToList();
        }

        private void FindTrades(ref List<Trade> allTrades, LeagueData leagueData,
                                TeamPlayerPool myTeamPlayerPool, TeamPlayerPool theirTeamPlayerPool,
                                IEnumerable<IEnumerable<Player>> myTradePool, IEnumerable<IEnumerable<Player>> theirTradePool)
        {
            IEnumerable<Trade> trades = from mySideOfTrade in myTradePool
                                        from theirSideOfTrade in theirTradePool
                                        select (new Trade(mySideOfTrade, theirSideOfTrade));

            List<Player> myRequiredPlayers = myTeamPlayerPool.TradablePlayers.Where(p => p.Required).ToList<Player>();
            List<Player> myExcludedPlayers = myTeamPlayerPool.TradablePlayers.Where(p => p.Excluded).ToList<Player>();
            List<Player> theirRequiredPlayers = theirTeamPlayerPool.TradablePlayers.Where(p => p.Required).ToList<Player>();
            List<Player> theirExcludedPlayers = theirTeamPlayerPool.TradablePlayers.Where(p => p.Excluded).ToList<Player>();

            foreach (Trade trade in trades)
            {
                if (Math.Abs(trade.Fairness) <= 5)
                {
                    trade.CalculateDifferentials(leagueData, myTeamPlayerPool, theirTeamPlayerPool);
                    if (trade.MyDifferential > 0 && trade.TheirDifferential > 0) {
                        bool addTrade = true;
                        
                        //see if my side has my required players
                        foreach (Player requiredPlayer in myRequiredPlayers)
                        {
                            bool foundPlayer = false;

                            foreach (Player player in trade.MyPlayers)
                            {
                                if (player.Id == requiredPlayer.Id) { foundPlayer = true; }
                            }

                            if (!foundPlayer) { addTrade = false; }
                        }

                        //see if their side has their required players
                        foreach (Player requiredPlayer in theirRequiredPlayers)
                        {
                            bool foundPlayer = false;

                            foreach (Player player in trade.TheirPlayers)
                            {
                                if (player.Id == requiredPlayer.Id) { foundPlayer = true; }
                            }

                            if (!foundPlayer) { addTrade = false; }
                        }

                        //see if my side doesn't have my excluded players
                        foreach (Player excludedPlayer in myExcludedPlayers)
                        {
                            foreach (Player player in trade.MyPlayers)
                            {
                                if (player.Id == excludedPlayer.Id) { addTrade = false; }
                            }
                        }

                        if (addTrade) { allTrades.Add(trade); }
                    }
                }
            }
        }

        private HasRequiredPlayers(List<Player> players, List<Player> requiredPlayers)
        {
            foreach (Player requiredPlayer in requiredPlayers)
            {
                bool foundPlayer = false;

                foreach (Player player in players)
                {
                    if (player.Id == requiredPlayer.Id) { foundPlayer = true; }
                }

                if (!foundPlayer) { addTrade = false; }
            }
        }

        private CheckExcludedPlayers()
        {

        }
    }
}