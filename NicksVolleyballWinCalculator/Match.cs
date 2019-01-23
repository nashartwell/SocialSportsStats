using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NicksVolleyballWinCalculator
{
    public class Match
    {
        public Match(string team1, string team1Score, string team2, string team2Score)
        {
            this.team1 = team1;
            this.team1Score = team1Score;
            this.team2 = team2;
            this.team2Score = team2Score;
        }


        public string team1 { get; set; }
        public string team1Score { get; set; }
        public string team2 { get; set; }
        public string team2Score { get; set; }
    }
}