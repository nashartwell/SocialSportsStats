using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using Glicko2;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace NicksVolleyballWinCalculator
{
    public partial class index : System.Web.UI.Page
    {
        List<Team> teamsGlobal = new List<Team>();
        List<Team> teams = new List<Team>();
        List<Match> matches = new List<Match>();

        RatingCalculator calculator = new RatingCalculator();
        RatingPeriodResults results = new RatingPeriodResults();

        protected void Page_Load(object sender, EventArgs e)
        {
            var  league = Request.QueryString["league"];
            if (!string.IsNullOrEmpty(league))
            {
                try
                {
                    if (league[league.Length - 1] == '/')
                        league = league.TrimEnd('/');
                    if (!league.Contains("app.mysocialsports.com"))
                        league = "https://app.mysocialsports.com/leagues/" + league;
                    doMath(league, false);
                }
                catch (Exception ex)
                {//856
                    doMath("https://app.mysocialsports.com/leagues/1070", true);
                    logging.Text = "Something went wrong, check League URL or if league has started (example url 'https://app.mysocialsports.com/leagues/501')" + "\n" + ex.ToString();
                }
            }
                else
                    doMath("https://app.mysocialsports.com/leagues/1070", false);
        }

        public void doMath(string url, bool clear)
        {
            logging.Text = "";
            League.Text = "";
            if(clear)
            {
                TeamTable.Rows.Clear();

                TableRow row = new TableRow();
                TableHeaderCell Name = new TableHeaderCell();
                TableHeaderCell Rating = new TableHeaderCell();
                Name.Text = "Name";
                Rating.Text = "Rating";

                Name.CssClass = "table";
                Rating.CssClass = "table";

                row.Cells.Add(Name);
                row.Cells.Add(Rating);
                row.Font.Size = FontUnit.Larger;
                
                TeamTable.Rows.Add(row);

                StandingsTable.Rows.Clear();
                MatchTable.Rows.Clear();
            }
            
            

            List<Match> matchesToCalc = new List<Match>();
            List<string> teamNames = new List<string>();
            int teamcount = 1;
            int scorecount = 1;
            int matchCount = 0;

            HtmlWeb web = new HtmlWeb();

            var docForTeamList = web.Load(url);
            var teamListNode = docForTeamList.DocumentNode.SelectNodes("//*[@id=\"sidebar\"]//div[3]/ul//li//a");
            var leagueNameNode = docForTeamList.DocumentNode.SelectNodes("/html//body//div[1]//div//div//div//div//div[1]//h3");
            League.Text = leagueNameNode[0].InnerText;
            foreach (HtmlNode tln in teamListNode)
            {
                teams.Add(new Team(tln.InnerText.Substring(1), new Rating(calculator)));
                teamNames.Add(tln.InnerText.Substring(1));
            }

            teamsGlobal = teams;
            
            var docForStandings = web.Load(url + "/standings");
            //var standingsNode = docForStandings.DocumentNode.SelectNodes("//html//body//div[1]//div//div[3]//div[1]");
            var standingsNode = docForStandings.DocumentNode.SelectNodes("//html//body//div[1]//div//div//div//div[2]//table");
            if (standingsNode[0].InnerText.Contains("League Home"))
                standingsNode = docForStandings.DocumentNode.SelectNodes("//html//body//div[1]//div/div[4]//div[1]");

            foreach(HtmlNode TRow in standingsNode[0].SelectNodes("//tr"))
            {
                TableRow row = new TableRow();
                TableCell text = new TableCell();

                string standingstring = Regex.Replace(TRow.InnerHtml, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1");
                text.Text = standingstring;


                row.Font.Size = FontUnit.Larger;

                row.Cells.Add(text);
                StandingsTable.Rows.Add(row);
            }

            var doc = web.Load(url + "/schedule");
            var teamNode = doc.DocumentNode.SelectNodes("//html//body//div//div//div//div//div//div[3]//table//td//a | //*[text() = 'Bye']");
            var schedual = doc.DocumentNode.SelectNodes("//html//body//div[1]//div//div//div//div//div[3]");
            var scoreNode = doc.DocumentNode.SelectNodes("//html//body//div//div//div//div//div//div//table//*[@id[starts-with(.,'hscore')] or @id[starts-with(.,'ascore')]]");
            
            for (int i = 0; i < teamNode.Count; i++)
            {
                if (teamNames.Contains(teamNode[i].InnerText) || teamNode[i].InnerText == "Bye")
                {
                    if (teamcount == 1)
                    {
                        Match temp = new Match("", "", "", "");
                        temp.team1 = teamNode[i].InnerText;
                        teamcount++;
                        matches.Add(temp);
                    }
                    else if (teamcount == 2)
                    {
                        matches[matches.Count - 1].team2 = teamNode[i].InnerText;
                        teamcount--;
                    }
                }
            }

            matches.RemoveAll(x => x.team1 == "Bye" || x.team2 == "Bye");

            for (int i = 0; i < scoreNode.Count; i++)
            {
                if (!String.IsNullOrWhiteSpace(scoreNode[i].InnerText))
                {
                    if (scorecount == 1)
                    {
                        matches[matchCount].team1Score = scoreNode[i].InnerText;
                        scorecount++;
                    }
                    else if (scorecount == 2)
                    {
                        matches[matchCount].team2Score = scoreNode[i].InnerText;
                        matchCount++;
                        scorecount--;
                    }
                }
            }
            
            goThroughMatches();

            //matches.Reverse();
            
            goThroughMatches();

            teams.Sort((x, y) => y.rating.GetRating().CompareTo(x.rating.GetRating()));

            foreach (Team tea in teams)
            {
                TableRow row = new TableRow();
                TableCell name = new TableCell();
                TableCell rating = new TableCell();

                name.CssClass = "table";
                rating.CssClass = "table";
                
                name.Text = tea.name;
                double rounded = Math.Round(tea.rating.GetRating(), 2);
                rating.Text = String.Format("{0:F2}", rounded);

                row.Font.Size = FontUnit.Larger;
                
                row.Cells.Add(name);
                row.Cells.Add(rating);
                TeamTable.Rows.Add(row);
            }

            foreach (HtmlNode Row in schedual[0].SelectNodes("//tr"))
            {
                if (!Row.ParentNode.ParentNode.OuterHtml.Contains("show-for-small-only"))
                {
                    TableRow row = new TableRow();
                    TableCell text = new TableCell();

                    string schedualstring = Regex.Replace(Row.InnerHtml, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1");
                    string schedualstringHome = Regex.Match(schedualstring, @"hscore_\d+..><.td>").Value;
                    string schedualstringAway = Regex.Match(schedualstring, @"ascore_\d+..><.td>").Value;
                    string teamFinder = Regex.Replace(Row.InnerText, @"\t", " ");
                    teamFinder = Regex.Replace(teamFinder, @"\n", " ");
                    

                    

                    string regexTeams = @"";
                    foreach (Team tea in teams)
                    {
                        regexTeams += tea.name + "|";
                    }
                    regexTeams = regexTeams.TrimEnd('|');

                    if (regexTeams.Contains("("))
                        regexTeams = regexTeams.Replace("(", @"\(");
                    if (regexTeams.Contains(")"))
                        regexTeams = regexTeams.Replace(")", @"\)");



                    MatchCollection teamsFound = Regex.Matches(teamFinder, regexTeams);

                    if (teamFinder.Contains("The Best Team"))
                    {

                        var j = "k";
                    }
                    if (teamsFound.Count >= 2)
                    {
                        int team1index = teams.FindIndex(x => x.name == teamsFound[0].Value);
                        int team2index = teams.FindIndex(x => x.name == teamsFound[1].Value);
                        string homewin = String.Format("{0:F2}", getPercent(teams[team1index], teams[team2index]));
                        string awaywin = String.Format("{0:F2}", (100-getPercent(teams[team1index], teams[team2index])));

                        schedualstringHome = schedualstringHome.Replace("><", ">" + homewin + "%<");
                        schedualstringAway = schedualstringAway.Replace("><", ">" + awaywin + "%<");
                    }
                    schedualstring = Regex.Replace(schedualstring, @"hscore_\d+..><.td>", schedualstringHome);
                    schedualstring = Regex.Replace(schedualstring, @"ascore_\d+..><.td>", schedualstringAway);

                    text.Text = schedualstring;

                    row.Font.Size = FontUnit.Larger;

                    row.Cells.Add(text);
                    MatchTable.Rows.Add(row);
                }
            }
        }

        protected void goThroughMatches()
        {
            foreach (Match ma in matches)
            {
                int team1index = 0, team2index = 0;

                team1index = teams.FindIndex(x => x.name == ma.team1);
                team2index = teams.FindIndex(x => x.name == ma.team2);

                if (!String.IsNullOrWhiteSpace(ma.team1Score) && !String.IsNullOrWhiteSpace(ma.team2Score))
                {
                    int team1score = Convert.ToInt32(ma.team1Score);
                    int team2score = Convert.ToInt32(ma.team2Score);

                    if (team1score == team2score)
                    {
                        for (int i = 0; i < team1score; i++)
                        {
                            results.AddDraw(teams[team2index].rating, teams[team1index].rating);
                        }
                    }
                    if (team1score > team2score)
                    {
                        for (int i = 0; i < team2score; i++)
                        {
                            calculateRatings(teams[team1index].rating, teams[team2index].rating);
                            calculateRatings(teams[team2index].rating, teams[team1index].rating);

                        }
                        for (int i = 0; i < (team1score - team2score); i++)
                        {
                            calculateRatings(teams[team1index].rating, teams[team2index].rating);
                        }
                    }
                    if (team2score > team1score)
                    {
                        for (int i = 0; i < team1score; i++)
                        {
                            calculateRatings(teams[team1index].rating, teams[team2index].rating);
                            calculateRatings(teams[team2index].rating, teams[team1index].rating);
                        }
                        for (int i = 0; i < (team2score - team1score); i++)
                        {
                            calculateRatings(teams[team2index].rating, teams[team1index].rating);
                        }
                    }
                }
            }
        }

        protected void calculateRatings(Rating winner, Rating loser)
        {
            results.AddResult(winner, loser);
            calculator.UpdateRatings(results);
        }

        protected double getPercent(Team team1, Team team2)
        {
            double number = 0;
            bool team2Better = false;
            if (team1.rating.GetRating() < team2.rating.GetRating())
                team2Better = true;

            number = (team1.rating.GetRatingDeviation() + team2.rating.GetRatingDeviation())*2;
            
            if (team2Better)
                number = (team2.rating.GetRating() - team1.rating.GetRating())/number;
            else
                number = (team1.rating.GetRating() - team2.rating.GetRating())/ number;
            double value = 0.0;
            Chart chart = new Chart();
            value = chart.DataManipulator.Statistics.NormalDistribution(number);
            value *= 100;
            value = Math.Round(value, 2);

            if (!team2Better)
                return value;
            else
                return 100 - value;
        }

        protected void urlBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string text = urlTextBox.Text;
                if (text[text.Length - 1] == '/')
                    text = text.TrimEnd('/');

                Response.Redirect("Index.aspx?league=" + urlTextBox.Text);
            }
            catch
            {
                logging.Text = "Something went wrong, check League URL (example url 'https://app.mysocialsports.com/leagues/501')";
            }
        }
    }
}