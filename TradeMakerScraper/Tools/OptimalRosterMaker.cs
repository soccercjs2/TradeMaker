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

            //add best waiver to team if missing starter
            if (roster.Quarterbacks.Count() < 1) { roster.Quarterbacks.Add(new RosterPlayer(leagueData.WaiverQuarterback)); }
            if (roster.RunningBacks.Count() < 2) { roster.RunningBacks.Add(new RosterPlayer(leagueData.WaiverRunningBack)); }
            if (roster.WideReceivers.Count() < 2) { roster.WideReceivers.Add(new RosterPlayer(leagueData.WaiverWideReceiver)); }
            if (roster.TightEnds.Count() < 1) { roster.TightEnds.Add(new RosterPlayer(leagueData.WaiverTightEnd)); }

            //get possible flex players
            Player flexRb = runningBacks.Skip(2).Take(1).FirstOrDefault();
            Player flexWr = wideReceivers.Skip(2).Take(1).FirstOrDefault();
            Player flexTe = tightEnds.Skip(1).Take(1).FirstOrDefault();

            //get flex points for easy comparing
            decimal flexRbPoints = (flexRb != null) ? flexRb.FantasyPoints : 0;
            decimal flexWrPoints = (flexWr != null) ? flexWr.FantasyPoints : 0;
            decimal flexTePoints = (flexTe != null) ? flexTe.FantasyPoints : 0;

            //add best flex player to starters and remove him from the bench
            Player flexPlayer;
            if (flexRbPoints > flexWrPoints && flexRbPoints > flexTePoints) { flexPlayer = flexRb; }
            else if (flexWrPoints > flexRbPoints && flexWrPoints > flexTePoints) { flexPlayer = flexWr; }
            else { flexPlayer = flexTe; }

            roster.Flexes.Add(new RosterPlayer(flexPlayer));
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