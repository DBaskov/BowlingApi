using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingApi.Common.CustomExceptions;
using BowlingApi.Services;
using BowlingApi.Services.Models;

namespace BowlingApi.BusinessLogicHelpers
{
    public class PlayersHelper : IPlayersHelper
    {
        enum SpecialScores { Spare = 10, Strike = 11}; 

        public readonly IPlayersDataService _playersDataService;
        public PlayersHelper(IPlayersDataService playersDataService)
        {
            _playersDataService = playersDataService;
        }

        public async Task<List<PlayerGameData>> ChangeFrameScore(string playerId, int frameNumber, int newScore)
        {
            throw new NotImplementedException();
        }
        
        public async Task<PlayerGameData> InstiateAndInsertPlayerGameData(string playerName)
        {
            var playerGameData = new PlayerGameData
            {
                PlayerId = Guid.NewGuid().ToString(),
                PlayerName = playerName
            };

            var success = await _playersDataService.AddPlayer(playerGameData);
            if (!success)
            {
                throw new MongoOperationFailException("Mongo 'AddPlayers' operation failed. ");
            }

            return playerGameData;
        } 

        public async Task<bool> DeletePlayerGameData(string playerId)
        {
            var result = await _playersDataService.DeletePlayer(playerId);

            return result;
        }

        public async Task<bool> DeleteBulkPlayerGameData(List<string> playerIds)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PlayerGameData>> InstatiateBulkPlayerGameData(List<string> playerNames)
        {
            var playersList = new List<PlayerGameData>();

            var matchId = Guid.NewGuid().ToString();
            foreach (var playerName in playerNames)
            {
                playersList.Add(
                    new PlayerGameData
                    {
                        PlayerId = Guid.NewGuid().ToString(),
                        MatchId = matchId,
                        PlayerName = playerName
                    });
            }

            var success = await _playersDataService.AddPlayers(playersList);
            if(!success)
            {
                throw new MongoOperationFailException("Mongo 'AddPlayers' operation failed. ");
            }

            return playersList;
        }        

        public async Task<PlayerGameData> UpdateScore(int numPins)
        {
            throw new NotImplementedException();
        }
    }
}
