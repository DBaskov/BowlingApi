using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bowling.Api.DTOs
{
    public class PlayerGameSessionIn
    {
        [Required]
        public virtual string PlayerName { get; set; }
        public int TotalScore { get; set; }

        public List<int> RunningTotalList { get; set; }

        public List<List<int>> ResultList { get; set; }
    }
}
