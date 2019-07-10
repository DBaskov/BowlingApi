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

        public string PlayerName { get; set; } = "";

        public int TotalScore { get; set; } = 0;

        public List<int> RunningTotalList { get; set; } = new List<int>();

        public List<int> ResultList { get; set; } = new List<int>();

        public FrameScores FrameScores { get; set; } = new FrameScores();
    }
}
