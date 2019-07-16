using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bowling.Api.DTOs
{
    public class StatusCheck
    {
        public string AppName { get; set; }

        public string Version { get; set; }

        public bool MongoDbWorking { get; set; }
    }
}
