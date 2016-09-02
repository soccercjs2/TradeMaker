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
        private const int FanDuelQbs = 1;
        private const int FanDuelRbs = 2;
        private const int FanDuelWrs = 3;
        private const int FanDuelTes = 1;
        private const int FanDuelKs = 1;
        private const int FanDuelDefs = 1;

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IEnumerable<DfsLineup> Post(DFSLineupPackage package)
        {
            //create lineup list to store each lineup 
            HashSet<DfsLineup> dfsLineups = new HashSet<DfsLineup>();

            IEnumerable<PlayerList> qbCombos = GetPositionCombos(package.Quarterbacks, FanDuelQbs);
            IEnumerable<PlayerList> rbCombos = GetPositionCombos(package.RunningBacks, FanDuelRbs);
            IEnumerable<PlayerList> wrCombos = GetPositionCombos(package.WideReceivers, FanDuelWrs);
            IEnumerable<PlayerList> teCombos = GetPositionCombos(package.TightEnds, FanDuelTes);
            IEnumerable<PlayerList> kCombos = GetPositionCombos(package.Kickers, FanDuelKs);
            IEnumerable<PlayerList> defCombos = GetPositionCombos(package.Defenses, FanDuelDefs);

            qbCombos = qbCombos.OrderBy(c => c.CostPerPoint).Take(15);
            rbCombos = rbCombos.OrderBy(c => c.CostPerPoint).Take(30);
            wrCombos = wrCombos.OrderBy(c => c.CostPerPoint).Take(45);
            teCombos = teCombos.OrderBy(c => c.CostPerPoint).Take(15);
            kCombos = kCombos.OrderBy(c => c.CostPerPoint).Take(15);
            defCombos = defCombos.OrderBy(c => c.CostPerPoint).Take(15);

            foreach (PlayerList qbCombo in qbCombos)
            {
                foreach (PlayerList rbCombo in rbCombos)
                {
                    foreach (PlayerList wrCombo in wrCombos)
                    {
                        foreach (PlayerList teCombo in teCombos)
                        {
                            foreach (PlayerList kCombo in kCombos)
                            {
                                foreach (PlayerList defCombo in defCombos)
                                {
                                    DfsLineup dfsLineup = new DfsLineup();
                                    dfsLineup.Quarterbacks = qbCombo;
                                    dfsLineup.RunningBacks = rbCombo;
                                    dfsLineup.WideReceivers = wrCombo;
                                    dfsLineup.TightEnds = teCombo;
                                    dfsLineup.Kickers = kCombo;
                                    dfsLineup.Defenses = defCombo;

                                    dfsLineup.Salary = qbCombo.Salary +
                                        rbCombo.Salary +
                                        wrCombo.Salary +
                                        teCombo.Salary +
                                        kCombo.Salary +
                                        defCombo.Salary;

                                    dfsLineup.FantasyPoints = qbCombo.FantasyPoints +
                                        rbCombo.FantasyPoints +
                                        wrCombo.FantasyPoints +
                                        teCombo.FantasyPoints +
                                        kCombo.FantasyPoints +
                                        defCombo.FantasyPoints;

                                    if ((package.SalaryCap * 0.99) < dfsLineup.Salary && dfsLineup.Salary < package.SalaryCap) //(package.SalaryCap * 0.95) < dfsLineup.Salary && 
                                    {
                                        dfsLineups.Add(dfsLineup);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            IEnumerable<DfsLineup> result = dfsLineups.OrderByDescending(l => l.FantasyPoints);
            return result;
        }

        private HashSet<PlayerList> GetPositionCombos(IEnumerable<Player> players, int numberOfPlayers)
        {
            HashSet<PlayerList> combos = new HashSet<PlayerList>();

            if (numberOfPlayers == 1) { combos = GetSinglePositionCombos(players); }
            if (numberOfPlayers == 2) { combos = GetDoublePositionCombos(players); }
            if (numberOfPlayers == 3) { combos = GetTriplePositionCombos(players); }

            return combos;
        }

        private HashSet<PlayerList> GetSinglePositionCombos(IEnumerable<Player> players)
        {
            IEnumerable<PlayerList> positionCombos = from firstPlayer in players
                                                     select new PlayerList() { Players = { firstPlayer } };

            HashSet<PlayerList> efficientPositionCombos = new HashSet<PlayerList>();

            foreach (PlayerList positionCombo in positionCombos)
            {
                foreach (Player player in positionCombo.Players)
                {
                    positionCombo.Salary += player.Salary;
                    positionCombo.FantasyPoints += player.FantasyPoints;
                }

                positionCombo.CostPerPoint = positionCombo.Salary / positionCombo.FantasyPoints;

                efficientPositionCombos.Add(positionCombo);
            }

            return efficientPositionCombos;
        }

        private HashSet<PlayerList> GetDoublePositionCombos(IEnumerable<Player> players)
        {
            IEnumerable<PlayerList> positionCombos = from firstPlayer in players
                                                      from secondPlayer in players
                                                      where firstPlayer != secondPlayer
                                                      select new PlayerList() { Players = { firstPlayer, secondPlayer } };

            HashSet<PlayerList> efficientPositionCombos = new HashSet<PlayerList>();

            foreach (PlayerList positionCombo in positionCombos)
            {
                foreach (Player player in positionCombo.Players)
                {
                    positionCombo.Salary += player.Salary;
                    positionCombo.FantasyPoints += player.FantasyPoints;
                }

                positionCombo.CostPerPoint = positionCombo.Salary / positionCombo.FantasyPoints;

                efficientPositionCombos.Add(positionCombo);
            }

            return efficientPositionCombos;
        }

        private HashSet<PlayerList> GetTriplePositionCombos(IEnumerable<Player> players)
        {
            IEnumerable<PlayerList> positionCombos = from firstPlayer in players
                                                      from secondPlayer in players
                                                      from thirdPlayer in players
                                                      where firstPlayer != secondPlayer && firstPlayer != thirdPlayer && secondPlayer != thirdPlayer
                                                      select new PlayerList() { Players = { firstPlayer, secondPlayer, thirdPlayer } };

            HashSet<PlayerList> efficientPositionCombos = new HashSet<PlayerList>();

            foreach (PlayerList positionCombo in positionCombos)
            {
                foreach (Player player in positionCombo.Players)
                {
                    positionCombo.Salary += player.Salary;
                    positionCombo.FantasyPoints += player.FantasyPoints;
                }

                positionCombo.CostPerPoint = positionCombo.Salary / positionCombo.FantasyPoints;

                efficientPositionCombos.Add(positionCombo);
            }

            return efficientPositionCombos;
        }
    }
}
