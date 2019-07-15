using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.DBContexts.Models
{
    public interface IPlayersMongoDbSetings
    {
        string DatabaseName { get; set; }

        string PlayersCollectionName { get; set; }

        string ConnectionString { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

        bool UseSSL { get; set; }

        int SocketConnectionTimeOutMs { get; set; }
    }
}
