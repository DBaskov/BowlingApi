using BowlingApi.DBContexts;
using BowlingApi.Services.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Services
{
    //todo try catch
    public class PlayersDataMongoService : IPlayersDataService
    {
        public IMongoDBContext _mongoDbContext { get; set; }

        public PlayersDataMongoService(IMongoDBContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        //think if you want to return whole playerGameData object
        public async Task<bool> AddPlayers(List<PlayerGameData> players)
        {
            try
            {
               await _mongoDbContext.PlayersMongoCollection.InsertManyAsync(players);
                return true;
            }
            catch(Exception)
            {
                //log exception here
                return false;
            }
        }

        public async Task<bool> DeletePlayers(List<string> playerIds)
        {

            var filter = Builders<PlayerGameData>.Filter.In(p => p.PlayerId, playerIds);

            var result = await _mongoDbContext.PlayersMongoCollection.DeleteManyAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> DeletePlayer(string playerId)
        {
            var filter = Builders<PlayerGameData>.Filter.Eq(p => p.PlayerId, playerId);

            var result = await _mongoDbContext.PlayersMongoCollection.DeleteManyAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<PlayerGameData> UpdatePlayerData(PlayerGameData updatedGameData)
        {
            throw new NotImplementedException();
        }

        public async Task<PlayerGameData> GetPlayerData(string playerId)
        {
            var filter = Builders<PlayerGameData>.Filter.Eq(p => p.PlayerId, playerId);

            var result = await _mongoDbContext.PlayersMongoCollection.FindAsync(filter);
            var playerInfo = result.FirstOrDefault();

            if(playerInfo == null)
            {
                throw new ItemNotFoundInMongoException("item with object id: " + playerId + " is not found in PlayersData Mongo Collection");
            }

            return playerInfo;
        }
    }
}
