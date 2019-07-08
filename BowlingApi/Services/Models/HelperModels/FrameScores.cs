using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Services.Models.HelperModels
{
    public class FrameScores
    {
        List<Frame> RegularFrames;
        Tuple<int, int, int> BonusFrame;
    }
}
