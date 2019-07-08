using BowlingApi.Services.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.DBContexts
{
    public interface IMongoDBContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<PlayerGameData> PlayersMongoCollection { get; }

        Task<bool> ConnectionOk { get; }
    }
}
