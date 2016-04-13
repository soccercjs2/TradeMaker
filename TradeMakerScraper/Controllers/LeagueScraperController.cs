using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Controllers
{
    public class LeagueScraperController : ApiController
    {
        // GET api/leaguescraper
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public LeagueData Get()
        {
            LeagueData leagueData = new LeagueData();

            Team broncos = new Team(1, "Broncos");
            Team seahawks = new Team(2, "Seahawks");

            leagueData.Teams.Add(broncos);
            leagueData.Teams.Add(seahawks);

            Player anderson = new Player(1, "C.J. Anderson", "DEN", "RB");
            Player sanchez = new Player(2, "Mark Sanchez", "DEN", "QB");
            Player sanders = new Player(3, "Emanuel Sanders", "DEN", "WR");

            Player lynch = new Player(4, "Marshawn Lynch", "SEA", "RB");
            Player wilson = new Player(5, "Russel Wilson", "SEA", "QB");
            Player baldwin = new Player(6, "Doug Baldwin", "SEA", "WR");

            broncos.Players.Add(anderson);
            broncos.Players.Add(sanchez);
            broncos.Players.Add(sanders);

            seahawks.Players.Add(lynch);
            seahawks.Players.Add(wilson);
            seahawks.Players.Add(baldwin);

            return leagueData;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public string Post(Projections projections)
        {
            return "asdf";
        }
    }
}
