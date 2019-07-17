
using BowlingApi.Repositories.Models.HelperModels;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repositories.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerGameData
    {
        public string PlayerId { get; set; }

        public string PlayerName { get; set; } = "";

        public int TotalScore { get; set; } = 0;

        public List<int> RunningTotalList { get; set; } = new List<int>();

        public List<Frame> ResultList { get; set; } = new List<Frame>();
   
    }
}
