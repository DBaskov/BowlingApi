using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.DTOs.HelperModels
{
    public class FrameScores
    {
        public List<List<int>> Frames { get; set; } = new List<List<int>>();

        //public Tuple<int, int, int> BonusFrame { get; set; } = new Tuple<int, int, int>(0, 0, 0);
    }
}
