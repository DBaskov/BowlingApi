using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repositories.Models.HelperModels
{
    [BsonIgnoreExtraElements]
    public class Frame
    {
        public List<int> ScoreCells { get; set; }

        public bool IsStrike { get; set; }

        public bool IsSpare { get; set; }
    }
}
