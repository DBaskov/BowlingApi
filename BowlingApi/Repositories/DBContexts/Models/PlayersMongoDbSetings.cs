using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.DBContexts.Models
{
    public class PlayersMongoDbSetings : IPlayersMongoDbSetings
    {
        public string DatabaseName { get; set; }
        public string PlayersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public int SocketConnectionTimeOutMs { get; set; }
    }
}
