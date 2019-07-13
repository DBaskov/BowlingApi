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

        public Tuple<int, int> GetScoreTwoShotsBackAndFrameNum(List<List<int>> resultList, int curFrameNum, int curCellNum)
        {
            if(curFrameNum < 2)
            {
                return new Tuple<int, int>(0, -1);
            }

            var frameNumResult = curFrameNum - 2; //zero indexed, frame before current
            if (curCellNum == 1)
            {
                return new Tuple<int, int>(resultList[frameNumResult][0], frameNumResult+1); //we have a strike ex [10][-1] [4][4]                
            }
            else
            {
                if (resultList[frameNumResult][0] == 10) // if 10 is in first cell, we have a strike [10][-1] [4], need to go back one more
                {
                    if (curFrameNum > 2)
                    {
                        frameNumResult -= 1; //one more frame back
                        return new Tuple<int, int>(resultList[frameNumResult][1], frameNumResult+1);
                    }
                    else
                    {
                        return new Tuple<int, int>(0, -1);
                    }
                }
                else
                {
                    return new Tuple<int, int>(resultList[frameNumResult][1], frameNumResult+1);
                }
            }
        }

        //returns the score and frame number
        //if it returns -1 then it's a strike
        public Tuple<int,int> GetScoreOneShotBackAndFrameNum(List<List<int>> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum == 1 && curCellNum == 1)
            {
                return new Tuple<int, int>(0, -1);
            }

            var frameNumResultIdx = curFrameNum - 1;
            if (curCellNum == 1)
            {
                return new Tuple<int, int>(resultList[frameNumResultIdx - 1][1], frameNumResultIdx);               
            }
            else
            {
                if(resultList[frameNumResultIdx][curCellNum-1] != 10)
                {                   
                     return new Tuple<int, int>(resultList[frameNumResultIdx][0], frameNumResultIdx + 1);                   
                }
                else //it's a spare
                {
                    frameNumResultIdx -= 1;
                    if (frameNumResultIdx >= 0)
                    {
                        return new Tuple<int, int>(resultList[frameNumResultIdx][1], frameNumResultIdx + 1);
                    }
                    else
                    {
                        return new Tuple<int, int>(0, -1);
                    }
                }               
            }
        }

        public void RecalculateRunningTotal(List<int> runningTotal, int newScore, int frameNum)
        {
            if(runningTotal.Count < 2)
            {
                return;
            }
            //todo additional logic to check if params are valid
            var frameIdx = frameNum - 1;
            var scoreDifference = newScore - runningTotal[frameIdx];
            runningTotal[frameIdx] = newScore;            

            for (var i = frameIdx+1; i < runningTotal.Count; i++)
            {
                runningTotal[i] += scoreDifference;
            }
        }

        public bool HandleStrike(PlayerGameData playerScore, int frameNum, int cellNum, int numPins)
        {
            var resultList = playerScore.ResultList;
            if (frameNum > 1)
            {
                var scoreAndFrameTwoShots = GetScoreTwoShotsBackAndFrameNum(resultList, frameNum, cellNum);

                if(scoreAndFrameTwoShots.Item1 == -1) {//it's a strike
                    var scoreAndFrameOneShot = GetScoreOneShotBackAndFrameNum(resultList, frameNum, cellNum);

                    var strikeFrameNewScore = scoreAndFrameTwoShots.Item1 + scoreAndFrameOneShot.Item1 + numPins; //expect strikes and spare to have score of 10
                    RecalculateRunningTotal(playerScore.RunningTotalList, strikeFrameNewScore, scoreAndFrameTwoShots.Item2);
                    playerScore.TotalScore = playerScore.RunningTotalList[frameNum - 1];
                    return true;
                }               
            }
            
            return false;           
        }

        public bool HandleSpare(PlayerGameData playerScore, int frameNum, int cellNum, int numPins)
        {
            var resultList = playerScore.ResultList;
            if (frameNum > 1)
            {
                var scoreAndFrameOneShot = GetScoreOneShotBackAndFrameNum(resultList, frameNum, cellNum);

                if (scoreAndFrameOneShot.Item1 == 10) //it's a spare
                {

                    var spareFrameNewScore = scoreAndFrameOneShot.Item1 + numPins; //expect strikes and spare to have score of 10
                    RecalculateRunningTotal(playerScore.RunningTotalList, spareFrameNewScore, scoreAndFrameOneShot.Item2);
                    playerScore.TotalScore = playerScore.RunningTotalList[frameNum - 1];
                    return true;
                }
            }

            return false;
        }

        private bool HandleStrikesAndSpares(PlayerGameData playerScore, int frameNum, int cellNum, int numPins)
        {

            return HandleStrike(playerScore, frameNum, cellNum, numPins) || HandleSpare(playerScore, frameNum, cellNum, numPins);
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
                frame = score.ResultList[frameNum - 1];

                if (frame.Count == 1) //also logic for 10
                {
                    score.RunningTotalList[frameNum - 1] = score.TotalScore;                    
                    frame.Add(numPins);
                }
                else
                {
                    frame = new List<int>() { numPins };
                    score.RunningTotalList.Add(score.TotalScore);
                    score.ResultList.Add(frame);
                }

                HandleStrikesAndSpares(score, score.ResultList.Count, frame.Count, numPins);
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
