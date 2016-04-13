using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TradeMakerScraper.Models;
using TradeMakerScraper.Tools;

namespace TradeMakerScraper.Controllers
{
    public class ProjectionScraperController : ApiController
    {
        // GET api/projectionscraper
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public Projections Get()
        {
            List<Player> players = new List<Player>();

            //get qb data
            string html = "";
            //Thread qbThread = new Thread(() => html = ScrapeBrowser(qbProjectionUrl));
            //qbThread.SetApartmentState(ApartmentState.STA);
            //qbThread.Start();
            //qbThread.Join();

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String path = Directory.GetCurrentDirectory() + "\\Html\\projections.html";
            html = File.ReadAllText(path);

            //create html object and load html into it
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            //get projection-data table from html
            HtmlNode projectionTable = document.GetElementbyId("projection-data");

            //loop through rows in projection-data
            foreach (HtmlNode row in projectionTable.SelectNodes("./tr"))
            {
                //create new datarow
                Player player = new Player();
                NumberFireParser numberFireParser = new NumberFireParser(row.SelectSingleNode("./td[1]").InnerText);

                //set row values
                player.Id = players.Count + 1;
                player.Name = numberFireParser.Player;
                player.Position = numberFireParser.Position;
                player.NflTeam = numberFireParser.Team;
                player.NflAlternateTeam = numberFireParser.AlternateTeam;
                player.PassingYards = decimal.Parse(row.SelectSingleNode("./td[5]").InnerText);
                player.PassingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[6]").InnerText);
                player.Interceptions = decimal.Parse(row.SelectSingleNode("./td[7]").InnerText);
                player.RushingYards = decimal.Parse(row.SelectSingleNode("./td[9]").InnerText);
                player.RushingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[10]").InnerText);
                player.Receptions = decimal.Parse(row.SelectSingleNode("./td[11]").InnerText);
                player.ReceivingYards = decimal.Parse(row.SelectSingleNode("./td[12]").InnerText);
                player.ReceivingTouchdowns = decimal.Parse(row.SelectSingleNode("./td[13]").InnerText);
                player.FantasyPoints = decimal.Parse(row.SelectSingleNode("./td[15]").InnerText);

                //add datarow to datatable
                players.Add(player);
            }

            Projections projections = new Projections();
            projections.Players = players;
            return projections;
        }
    }
}
