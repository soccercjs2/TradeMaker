using System;
using System.Collections.Generic;
using System.IO;
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
            HttpWebRequest webRequest;
            StreamReader responseReader;
            string responseData;
            CookieContainer cookies = new CookieContainer();
            StreamWriter requestWriter;

            //string postData = string.Format(null, team.League.UserName, team.League.Password);

            try
            {
                //get login  page with cookies
                webRequest = (HttpWebRequest)WebRequest.Create(team.League.LeagueHost.LoginUrl);
                webRequest.CookieContainer = cookies;

                //recieve non-authenticated cookie
                webRequest.GetResponse().Close();

                //post form  data to page
                webRequest = (HttpWebRequest)WebRequest.Create(team.League.LeagueHost.LoginUrl);
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.CookieContainer = cookies;
                webRequest.ContentLength = postData.Length;

                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(postData);
                requestWriter.Close();

                //recieve authenticated cookie
                webRequest.GetResponse().Close();

                //now we get the authenticated page
                webRequest = (HttpWebRequest)WebRequest.Create(team.Url);
                webRequest.CookieContainer = cookies;
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
                responseReader.Close();

                //load html into htmlagilitypack
                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(responseData);

                HtmlNode table = document.GetElementbyId(League.LeagueHost.StarterTableName);
                foreach (Player player in Players)
                {
                    if (table.InnerHtml.ToLower().Contains(player.Name.ToLower()) && table.InnerHtml.ToLower().Contains(player.Position.ToLower()) &&
                            (table.InnerHtml.ToLower().Contains(player.NflTeam.ToLower()) || table.InnerHtml.ToLower().Contains(player.NflAlternateTeam.ToLower())))
                    {
                        try
                        {
                            player.TeamId = team.TeamId;
                            //db.SaveChanges();
                        }
                        catch (Exception e) { }
                    }
                }
            }
            catch { }

            return "asdf";
        }
    }
}
