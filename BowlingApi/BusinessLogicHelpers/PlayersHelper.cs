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

        public async Task<PlayerGameData> UpdateScore(Guid playerId, int numPins)
        {
            var score = await _playersDataService.GetPlayerData(playerId.ToString());

            score.TotalScore += numPins;

            var frame = new List<int>();
            //var curFrameIdx = 0;
           // var curCellIdx = 0;

            var frameNum = score.ResultList.Count;
            if (frameNum > 0) //optimize this later
            {
                frame = score.ResultList[score.ResultList.Count - 1];

                if (frame.Count == 1) //also logic for 10
                {
                    score.RunningTotalList[score.ResultList.Count - 1] = score.TotalScore;                    
                    frame.Add(numPins);
                }
                else
                {
                    score.RunningTotalList.Add(score.TotalScore);
                    score.ResultList.Add(new List<int>() { numPins });
                }

                if(frameNum)
            }
            else
            {
                score.RunningTotalList.Add(score.TotalScore);
                score.ResultList.Add(new List<int>() { numPins });
            }
            
            /*
            if (prevFrameIdx < 10) //check this logic
            {
                if(prevCellIdx == 0)
                {
                    score.RunningTotalList[prevFrameIdx] = score.TotalScore;
                    score.ResultList[prevFrameIdx][prevCellIdx + 1] = numPins;
                }
                else
                {
                    score.RunningTotalList.Add(score.TotalScore); //what about strike
                    score.ResultList.Add(new List<int>() { numPins });
                }
            }
            else if(prevFrameIdx == 10)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            } */

            await _playersDataService.UpdatePlayerData(score);

            return score;
        }
    }
}
