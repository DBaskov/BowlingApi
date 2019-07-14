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

        //todo simply found strike/spare logic
        public Tuple<bool,int> FoundStrikeTwoShotsBackAndFrameNum(List<List<int>> resultList, int curFrameNum, int curCellNum)
        {
            if(curFrameNum < 2)
            {
                return new Tuple<bool, int>(false, -1);
            }

            var frameNumResult = curFrameNum - 2; //zero indexed, frame before current
            if (curCellNum == 1)
            {               
                if (resultList[frameNumResult][0] == 10) //found a strike one cell back
                {
                    if (frameNumResult > 0)
                    {                       
                        frameNumResult -= 1;
                        if (resultList[frameNumResult][1] == -1)
                        {
                            return new Tuple<bool, int>(true, frameNumResult + 1);
                        }                        
                    }                    
                }
                return new Tuple<bool, int>(false, -1);
            }
            else
            {
                if (resultList[frameNumResult][1] == -1) // if 10 is in first cell, we have a strike [10][-1] [4], need to go back one more
                {                   
                    return new Tuple<bool, int>(true, frameNumResult+1);                                     
                }               
                return new Tuple<bool, int>(false, -1);               
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
                frameNumResultIdx -= 1;
                return new Tuple<int, int>(resultList[frameNumResultIdx][1], frameNumResultIdx + 1);               
            }
            else
            {          
                return new Tuple<int, int>(resultList[frameNumResultIdx][0], frameNumResultIdx + 1);                                                                 
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
                var foundStrikeAndFrame = FoundStrikeTwoShotsBackAndFrameNum(resultList, frameNum, cellNum);

                if (foundStrikeAndFrame.Item1)
                {
                    var shotBeforeResult = GetScoreOneShotBackAndFrameNum(resultList, frameNum, cellNum);

                    var shotBefore = shotBeforeResult.Item1;
                    if (shotBeforeResult.Item1 == -1)
                    {
                        shotBefore = 10;
                    }

                    var newScore = shotBefore + numPins + 10; //spares are handled

                    RecalculateRunningTotal(playerScore.RunningTotalList, newScore, foundStrikeAndFrame.Item2);
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

                    var spareFrameNewScore = playerScore.RunningTotalList[scoreAndFrameOneShot.Item2-1] + numPins; //expect strikes and spare to have score of 10
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
            var frame = new List<int>() { numPins };

            if (score.ResultList.Count == 0) //perhaps move this to models instatiation
            {
                score.RunningTotalList.Add(score.TotalScore);
                frame = new List<int>() { numPins };
                score.ResultList.Add(frame);
                if (numPins == 10)
                {
                    frame.Add(-1);
                }

            }
            else
            {

                frame = score.ResultList[score.ResultList.Count - 1];
                var cellNum = frame.Count;
                if (cellNum > 0) //optimize this later
                {

                    if (cellNum == 1) //also logic for 10
                    {
                        score.RunningTotalList[score.ResultList.Count - 1] = score.TotalScore;
                        frame.Add(numPins);
                        cellNum++;
                    }
                    else
                    {
                        frame = new List<int>() { numPins };
                        cellNum = frame.Count;
                        score.RunningTotalList.Add(score.TotalScore);
                        if (numPins == 10)
                        {
                            frame.Add(-1);
                        }
                        score.ResultList.Add(frame);
                    }
                }
                else
                {
                    score.RunningTotalList.Add(score.TotalScore);
                    score.ResultList.Add(new List<int>() { numPins });
                    cellNum = 1;
                }

                HandleStrikesAndSpares(score, score.ResultList.Count, cellNum, numPins);
            }
            
            await _playersDataService.UpdatePlayerData(score);

            return score;
        }
    }
}
