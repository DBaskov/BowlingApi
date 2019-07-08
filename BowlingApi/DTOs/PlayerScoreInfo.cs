using BowlingApi.DTOs.HelperModels;
using System.Collections.Generic;

namespace BowlingApi.DTOs
{
    public class PlayerScoreInfo
    {
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }

        public List<int> RunningTotalList { get; set; }

        public List<int> ResultList { get; set; }

        public FrameScores FrameScores { get; set; }
    }
}
