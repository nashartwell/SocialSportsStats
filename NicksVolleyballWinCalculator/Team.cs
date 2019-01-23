using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glicko2;

namespace NicksVolleyballWinCalculator
{
    public class Team
    {
        public Team(string name, Rating rating)
        {
            this.name = name;
            this.rating = rating;
        }


        public string name { get; set; }
        public Rating rating { get; set; }
    }
}