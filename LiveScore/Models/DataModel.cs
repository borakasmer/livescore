using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveScore.Models
{
    public class MatchData
    {
        public int ID { get; set; }
        public string Team1Name { get; set; }
        public string Team2Name { get; set; }
        public string Team1Flag { get; set; }
        public string Team2Flag { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }

    }
}