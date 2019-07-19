﻿
using System.Collections.Generic;

namespace BowlingApi.DTOs
{
    public class PlayerGameDataOut
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }

        public List<int> RunningTotalList { get; set; }

        public List<List<int>> ResultList { get; set; }
    }
}
