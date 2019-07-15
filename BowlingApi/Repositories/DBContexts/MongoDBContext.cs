using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingApi.DBContexts.Models;
using BowlingApi.Repositories.Models;
using MongoDB.Driver;

namespace BowlingApi.DBContexts
{
    public class MongoDBContext : IMongoDBContext
    {
        public readonly IMongoDatabase _database;
        public readonly IMongoCollection<PlayerGameData> _playersCollection;

        public MongoDBContext(IPlayersMongoDbSetings mongoDbSettings)
        {
            var client = GetDatabaseClient(mongoDbSettings);
            _database = client.GetDatabase(mongoDbSettings.DatabaseName);
            _playersCollection = _database.GetCollection<PlayerGameData>(mongoDbSettings.PlayersCollectionName);
        }

        private MongoClient GetDatabaseClient(IPlayersMongoDbSetings mongoDbSettings)
        {
            var mongoSettings = new MongoClientSettings();

            mongoSettings.UseSsl = mongoDbSettings.UseSSL;
            mongoSettings.SocketTimeout = new TimeSpan(0, 0, 0, 0, mongoDbSettings.SocketConnectionTimeOutMs);
            mongoSettings.SslSettings = new SslSettings { CheckCertificateRevocation = true };
            if (!string.IsNullOrEmpty(mongoDbSettings.UserName) && !string.IsNullOrEmpty(mongoDbSettings.Password))
            {
                mongoSettings.Credential = MongoCredential.CreateCredential(mongoDbSettings.DatabaseName, mongoDbSettings.UserName, mongoDbSettings.Password);
            }

            return new MongoClient(mongoSettings);
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<PlayerGameData> PlayersMongoCollection => _playersCollection;

        public Task<bool> ConnectionOk => throw new NotImplementedException();

    }
}
