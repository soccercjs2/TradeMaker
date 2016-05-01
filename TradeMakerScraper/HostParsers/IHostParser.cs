using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.HostParsers
{
    public interface IHostParser
    {
        string GetLoginUrl();

        string GetPostData(string username, string password);

        void ParseLeague(HtmlDocument document, ref LeagueData leagueData);

        void ParseTeam(HtmlDocument document, League league, Team team, Projections projections);
    }
}