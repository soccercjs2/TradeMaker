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
            WebScraper scraper = null;
            IHostParser parser = null;

            //determine which parser to use based on url
            if (package.League.Url.Contains("www.fleaflicker.com")) { parser = new FleaflickerLeagueParser(); }
            else
            { 
                //throw exceptions saying league host not supported
            }

            //load web scraper based on whether a login is required
            if (package.League.RequiresLogin)
            {
                string loginUrl = parser.GetLoginUrl();
                string postData = parser.GetPostData(package.League.Username, package.League.Password);
                scraper = new WebScraper(loginUrl, postData);
            }
            else { scraper = new WebScraper(); }

            //parse league page to create teams in league data
            parser.ParseLeague(scraper.Scrape(package.League.Url), ref leagueData);

            //scrape each team and parse into league data
            foreach (Team team in leagueData.Teams)
            {
                parser.ParseTeam(scraper.Scrape(team.Url), team, package.Projections);
            }

            leagueData.WaiverQuarterback = package.Projections.Players.Where(p => p.Position == "QB").OrderByDescending(p => p.FantasyPoints).First<Player>();
            leagueData.WaiverRunningBack = package.Projections.Players.Where(p => p.Position == "RB").OrderByDescending(p => p.FantasyPoints).First<Player>();
            leagueData.WaiverWideReceiver = package.Projections.Players.Where(p => p.Position == "WR").OrderByDescending(p => p.FantasyPoints).First<Player>();
            leagueData.WaiverTightEnd = package.Projections.Players.Where(p => p.Position == "TE").OrderByDescending(p => p.FantasyPoints).First<Player>();

            foreach (Team team in leagueData.Teams)
            {
                foreach (Player player in team.Players)
                {
                    if (player.Position == "QB") { player.TradeValue = player.FantasyPoints - leagueData.WaiverQuarterback.FantasyPoints; }
                    if (player.Position == "RB") { player.TradeValue = player.FantasyPoints - leagueData.WaiverRunningBack.FantasyPoints; }
                    if (player.Position == "WR") { player.TradeValue = player.FantasyPoints - leagueData.WaiverWideReceiver.FantasyPoints; }
                    if (player.Position == "TE") { player.TradeValue = player.FantasyPoints - leagueData.WaiverTightEnd.FantasyPoints; }
                }
            }

            return leagueData;
        }
    }
}
