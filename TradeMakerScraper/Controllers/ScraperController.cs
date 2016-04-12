using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace TradeMakerScraper.Controllers
{
    public class ScraperController : ApiController
    {
        // GET api/scraper
        [EnableCors(origins: "http://trademaker.azurewebsites.net", headers: "*", methods: "*")]
        public string Get()
        {
            return "You got the api value!";
        }
    }
}
