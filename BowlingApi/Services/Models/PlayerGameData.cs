using BowlingApi.DTOs.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Services.Models
{
    public class PlayerGameData
    {
        public string PlayerId { get; set; }

        public string MatchId { get; set; }

        public string PlayerName { get; set; }

        public int TotalScore { get; set; }

        public List<int> RunningTotalList { get; set; }

        public List<int> ResultList { get; set; }

        public FrameScores FrameScores { get; set; }
    }
}
