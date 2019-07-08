using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.DTOs
{
    public class ScoresOnCurrentFrameOut
    {
        public string PlayerName { get; set; }

        public int Result { get; set; }

        public int Total { get; set; }
    }
}
