using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TradeMakerScraper.Models
{
    public class LeagueData
    {
        public League League { get; set; }
        public List<Team> Teams { get; set; }
        public HashSet<Player> Waivers { get; set; }
        public Team MyTeam { get; set; }
        public Team TheirTeam { get; set; }
        public bool UseAllTeams { get; set; }
        public List<Trade> Trades { get; set; }

        public LeagueData()
        {
            Teams = new List<Team>();
            Trades = new List<Trade>();
        }

        public Player GetWaiver(string position, int skip)
        {
            if (Waivers != null && Waivers.Count > skip)
            {
                List<Player> players = Waivers.Where(p => p.Position == position).ToList<Player>();
                return players.Skip(skip).Take(1).FirstOrDefault<Player>();
            }

            return null;
        }

        public Player GetWaiver(int skippedQuarterbacks, int skippedRunningBacks, int skippedWideReceivers, int skippedTightEnds)
        {
            Player quarterback = GetWaiver("QB", skippedQuarterbacks);
            Player runningBack = GetWaiver("RB", skippedQuarterbacks);
            Player wideReceiver = GetWaiver("WR", skippedQuarterbacks);
            Player tightEnd = GetWaiver("TE", skippedQuarterbacks);

            if (runningBack.FantasyPoints >= wideReceiver.FantasyPoints && runningBack.FantasyPoints >= tightEnd.FantasyPoints) { return runningBack; }
            else if (wideReceiver.FantasyPoints >= runningBack.FantasyPoints && wideReceiver.FantasyPoints >= tightEnd.FantasyPoints) { return runningBack; }
            else if (tightEnd.FantasyPoints >= runningBack.FantasyPoints && tightEnd.FantasyPoints >= wideReceiver.FantasyPoints) { return runningBack; }
            else { return null; }
        }
    }
}