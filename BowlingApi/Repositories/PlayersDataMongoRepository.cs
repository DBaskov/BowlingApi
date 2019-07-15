using BowlingApi.Common.CustomExceptions;
using BowlingApi.DBContexts;
using BowlingApi.Repositories.Models;
using BowlingApi.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repositories
{
    //todo try catch
    public class PlayersDataMongoRepository : IPlayersDataRepository
    {
        public IMongoDBContext _mongoDbContext { get; set; }

        public PlayersDataMongoRepository(IMongoDBContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<bool> AddPlayer(PlayerGameData player)
        {
            try
            {
                await _mongoDbContext.PlayersMongoCollection.InsertOneAsync(player);
                return true;
            }
            catch (Exception)
            {
                //log exception here
                return false;
            }
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

            try
            {
                var result = await _mongoDbContext.PlayersMongoCollection.DeleteOneAsync(filter);

                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch(Exception e) {
                throw;
            }
        }

        //curently only updates score data, also need to double check the updates
        public async Task<bool> UpdatePlayerData(PlayerGameData updatedGameData) 
        {
            var filter = Builders<PlayerGameData>.Filter.Eq(p => p.PlayerId, updatedGameData.PlayerId);

            var updateDef = Builders<PlayerGameData>.Update.
                Set(p => p.TotalScore, updatedGameData.TotalScore).
                Set(p => p.ResultList, updatedGameData.ResultList).
                Set(p => p.RunningTotalList, updatedGameData.RunningTotalList);
            try
            {
              var result = await  _mongoDbContext.PlayersMongoCollection.UpdateOneAsync(filter, updateDef);
              return result.IsAcknowledged && result.MatchedCount > 0;
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public async Task<PlayerGameData> GetPlayerData(string playerId)
        {
            var filter = Builders<PlayerGameData>.Filter.Eq(p => p.PlayerId, playerId);

            try
            {
                var result = await _mongoDbContext.PlayersMongoCollection.FindAsync(filter);

                var playerInfo = result.FirstOrDefault();

                if (playerInfo == null)
                {
                    throw new ItemNotFoundInMongoException("item with object id: " + playerId + " is not found in PlayersData Mongo Collection");
                }
                return playerInfo;
            }
            catch(Exception e)
            {
                throw;
            }            
        }

        public async Task<bool> ReplacePlayerData(PlayerGameData playerGameDataToReplace)
        {
            var filter = Builders<PlayerGameData>.Filter.Eq(p => p.PlayerId, playerGameDataToReplace.PlayerId);

            try
            {
                var result = await _mongoDbContext.PlayersMongoCollection.ReplaceOneAsync(filter, playerGameDataToReplace);
                return result.IsAcknowledged && result.MatchedCount > 0;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
