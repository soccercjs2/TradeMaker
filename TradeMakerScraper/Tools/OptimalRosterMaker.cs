using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradeMakerScraper.Models;

namespace TradeMakerScraper.Tools
{
    public class OptimalRosterMaker
    {
        public OptimalRosterMaker() { }

        public Roster GetRoster(LeagueData leagueData, List<Player> team, IEnumerable<Player> lostPlayers)
        {
            Roster roster = FindOptimalRoster(leagueData, team);
            roster = MarkLostPlayers(roster, lostPlayers);

            return roster;
        }

        public Roster GetRoster(LeagueData leagueData, List<Player> team, IEnumerable<Player> gainedPlayers, IEnumerable<Player> lostPlayers)
        {
            if (lostPlayers != null) { foreach (Player lostPlayer in lostPlayers) { team.Remove(lostPlayer); } }
            if (gainedPlayers != null) { foreach (Player gainedPlayer in gainedPlayers) { team.Add(gainedPlayer); } }

            Roster roster = FindOptimalRoster(leagueData, team);
            roster = MarkGainedPlayers(roster, gainedPlayers);

            return roster;
        }

        public Roster FindOptimalRoster(LeagueData leagueData, List<Player> team)
        {
            //create list of starters to return
            Roster roster = new Roster();

            //get position players
            List<Player> quarterbacks = team.Where(p => p.Position == "QB").OrderByDescending(p => p.FantasyPoints).ToList();
            List<Player> runningBacks = team.Where(p => p.Position == "RB").OrderByDescending(p => p.FantasyPoints).ToList();
            List<Player> wideReceivers = team.Where(p => p.Position == "WR").OrderByDescending(p => p.FantasyPoints).ToList();
            List<Player> tightEnds = team.Where(p => p.Position == "TE").OrderByDescending(p => p.FantasyPoints).ToList();

            //get starting players
            foreach (Player player in quarterbacks.Take(1).ToList()) { roster.Quarterbacks.Add(new RosterPlayer(player)); };
            foreach (Player player in runningBacks.Take(2).ToList()) { roster.RunningBacks.Add(new RosterPlayer(player)); };
            foreach (Player player in wideReceivers.Take(2).ToList()) { roster.WideReceivers.Add(new RosterPlayer(player)); };
            foreach (Player player in tightEnds.Take(1).ToList()) { roster.TightEnds.Add(new RosterPlayer(player)); };

            //add best waivers to team if missing starters
            int neededQuarterbacks = leagueData.League.Quarterbacks - roster.Quarterbacks.Count;
            int neededRunningBacks = leagueData.League.RunningBacks - roster.RunningBacks.Count;
            int neededWideReceivers = leagueData.League.WideReceivers - roster.WideReceivers.Count;
            int neededTightEnds = leagueData.League.TightEnds - roster.TightEnds.Count;

            for (int i = 0; i < neededQuarterbacks; i++) { roster.Quarterbacks.Add(new RosterPlayer(leagueData.GetWaiver("QB", i), true)); }
            for (int i = 0; i < neededRunningBacks; i++) { roster.RunningBacks.Add(new RosterPlayer(leagueData.GetWaiver("RB", i), true)); }
            for (int i = 0; i < neededWideReceivers; i++) { roster.WideReceivers.Add(new RosterPlayer(leagueData.GetWaiver("WR", i), true)); }
            for (int i = 0; i < neededTightEnds; i++) { roster.TightEnds.Add(new RosterPlayer(leagueData.GetWaiver("TE", i), true)); }

            //get possible flex players
            List<Player> possibleFlexes = new List<Player>();
            possibleFlexes.AddRange(runningBacks.Skip(leagueData.League.RunningBacks));
            possibleFlexes.AddRange(wideReceivers.Skip(leagueData.League.WideReceivers));
            possibleFlexes.AddRange(tightEnds.Skip(leagueData.League.TightEnds));
            possibleFlexes = possibleFlexes.OrderByDescending(p => p.FantasyPoints).ToList<Player>();

            //add remaining players on roster as flex

            for (int i = 0; i < leagueData.League.Flexes; i++)
            {
                if (possibleFlexes.Count > i) { roster.Flexes.Add(new RosterPlayer(possibleFlexes[i])); }
            }

            //find out if you need waivers for flex
            int neededFlexes = leagueData.League.Flexes - roster.Flexes.Count;
            int skippedQuarterbacks = neededQuarterbacks;
            int skippedRunningBacks = neededRunningBacks;
            int skippedWideReceivers = neededWideReceivers;
            int skippedTightEnds = neededTightEnds;

            //add waivers to flex
            for (int i = 0; i < neededFlexes; i++)
            {
                Player player = leagueData.GetWaiver(skippedQuarterbacks, skippedRunningBacks, skippedWideReceivers, skippedTightEnds);

                if (player.Position == "QB") { skippedQuarterbacks++; }
                if (player.Position == "RB") { skippedRunningBacks++; }
                if (player.Position == "WR") { skippedWideReceivers++; }
                if (player.Position == "TE") { skippedTightEnds++; }

                roster.Flexes.Add(new RosterPlayer(player, true));
            }

            //calculate roster point total
            roster.Points =
                roster.Quarterbacks.Sum(p => p.Player.FantasyPoints) +
                roster.RunningBacks.Sum(p => p.Player.FantasyPoints) +
                roster.WideReceivers.Sum(p => p.Player.FantasyPoints) +
                roster.TightEnds.Sum(p => p.Player.FantasyPoints) +
                roster.Flexes.Sum(p => p.Player.FantasyPoints);

            return roster;
        }

        public Roster MarkLostPlayers(Roster roster, IEnumerable<Player> lostPlayers)
        {
            //find quarterbacks lost
            foreach (RosterPlayer quarterback in roster.Quarterbacks)
            {
                Player lostPlayer = lostPlayers.Where(p => p.Id == quarterback.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { quarterback.OldPlayer = true; }
            }

            //find runningbacks lost
            foreach (RosterPlayer runningback in roster.RunningBacks)
            {
                Player lostPlayer = lostPlayers.Where(p => p.Id == runningback.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { runningback.OldPlayer = true; }
            }

            //find widereceivers lost
            foreach (RosterPlayer widereceiver in roster.WideReceivers)
            {
                Player lostPlayer = lostPlayers.Where(p => p.Id == widereceiver.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { widereceiver.OldPlayer = true; }
            }

            //find tightends lost
            foreach (RosterPlayer tightend in roster.TightEnds)
            {
                Player lostPlayer = lostPlayers.Where(p => p.Id == tightend.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { tightend.OldPlayer = true; }
            }

            //find flexes lost
            foreach (RosterPlayer flex in roster.Flexes)
            {
                Player lostPlayer = lostPlayers.Where(p => p.Id == flex.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { flex.OldPlayer = true; }
            }

            return roster;
        }

        public Roster MarkGainedPlayers(Roster roster, IEnumerable<Player> gainedPlayers)
        {
            //find quarterbacks gained
            foreach (RosterPlayer quarterback in roster.Quarterbacks)
            {
                Player lostPlayer = gainedPlayers.Where(p => p.Id == quarterback.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { quarterback.NewPlayer = true; }
            }

            //find runningbacks gained
            foreach (RosterPlayer runningback in roster.RunningBacks)
            {
                Player lostPlayer = gainedPlayers.Where(p => p.Id == runningback.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { runningback.NewPlayer = true; }
            }

            //find widereceivers gained
            foreach (RosterPlayer widereceiver in roster.WideReceivers)
            {
                Player lostPlayer = gainedPlayers.Where(p => p.Id == widereceiver.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { widereceiver.NewPlayer = true; }
            }

            //find tightends gained
            foreach (RosterPlayer tightend in roster.TightEnds)
            {
                Player lostPlayer = gainedPlayers.Where(p => p.Id == tightend.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { tightend.NewPlayer = true; }
            }

            //find flexes gained
            foreach (RosterPlayer flex in roster.Flexes)
            {
                Player lostPlayer = gainedPlayers.Where(p => p.Id == flex.Player.Id).FirstOrDefault<Player>();
                if (lostPlayer != null) { flex.NewPlayer = true; }
            }

            return roster;
        }
    }
}