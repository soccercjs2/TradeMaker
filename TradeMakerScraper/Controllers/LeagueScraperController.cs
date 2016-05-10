using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TradeMakerScraper.HostParsers;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Controllers
{
    public class LeagueScraperController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public LeagueData Post(LeagueScraperPackage package)
        {
            LeagueData leagueData = new LeagueData();
            leagueData.League = package.League;

            WebScraper scraper = null;
            IHostParser parser = null;

            foreach (Player player in package.Projections.Players)
            {
                player.FantasyPoints = new FantasyPointsCalculator(package.League, player).FantasyPoints;
            }

            //determine which parser to use based on url
            if (package.League.Url.Contains("www.fleaflicker.com")) { parser = new FleaflickerLeagueParser(); }
            else if (package.League.Url.Contains("www60.myfantasyleague.com")) { parser = new MFLParser(); }
            else if (package.League.Url.Contains("games.espn.go.com")) { parser = new EspnParser(); }
            else if (package.League.Url.Contains("football.fantasysports.yahoo.com")) { parser = new YahooParser(); }
            else
            { 
                //throw exceptions saying league host not supported
            }

            //load web scraper based on whether a login is required
            if (package.League.RequiresLogin)
            {
                string loginUrl = parser.GetLoginUrl();
                //parser.GetPostData(package.Username, package.Password);
                string postData = parser.GetPostData(null, null);
                scraper = new WebScraper(package.League.Url, loginUrl, postData);
            }
            else { scraper = new WebScraper(); }

            //parse league page to create teams in league data
            parser.ParseLeague(scraper.Scrape(package.League.Url), ref leagueData);

            //scrape each team and parse into league data
            foreach (Team team in leagueData.Teams)
            {
                parser.ParseTeam(scraper.Scrape(team.Url), package.League, team, package.Projections);
            }

            leagueData.Waivers = package.Projections.Players;
            Player waiverQuarterback = leagueData.GetWaiver("QB", 0);
            Player waiverRunningBack = leagueData.GetWaiver("RB", 0);
            Player waiverWideReceiver = leagueData.GetWaiver("WR", 0);
            Player waiverTightEnd = leagueData.GetWaiver("TE", 0);

            foreach (Team team in leagueData.Teams)
            {
                foreach (Player player in team.Players)
                {
                    if (player.Position == "QB") { player.TradeValue = player.FantasyPoints - waiverQuarterback.FantasyPoints; }
                    if (player.Position == "RB") { player.TradeValue = player.FantasyPoints - waiverRunningBack.FantasyPoints; }
                    if (player.Position == "WR") { player.TradeValue = player.FantasyPoints - waiverWideReceiver.FantasyPoints; }
                    if (player.Position == "TE") { player.TradeValue = player.FantasyPoints - waiverTightEnd.FantasyPoints; }
                }
            }

            return leagueData;
        }
    }
}
