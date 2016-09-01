using System;
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
    public class DFSLineupController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public List<DfsLineup> Post(DFSLineupPackage leagueData)
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

            return trades.OrderByDescending(t => ((t.MyDifferential + leagueData.Fairness) * (t.TheirDifferential - leagueData.Fairness))).Distinct().ToList();
        }
    }
}
