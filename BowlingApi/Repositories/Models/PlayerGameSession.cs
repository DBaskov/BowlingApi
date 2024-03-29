﻿
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repositories.Models
{
    [BsonIgnoreExtraElements]
    public class PlayerGameSession
    {
        public string PlayerGameSessionId { get; set; }

        public string PlayerName { get; set; } = "";

        public int TotalScore { get; set; } = 0;

        public List<int> RunningTotalList { get; set; } = new List<int>();

        public List<List<int>> ResultList { get; set; } = new List<List<int>>();
   
    }
}
