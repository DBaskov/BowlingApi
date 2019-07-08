using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.DTOs.HelperModels
{
    public class FrameScores
    {
        public List<Frame> Frames { get; set; }

        public Tuple<int, int, int> BonusFrame { get; set; }
    }
}
