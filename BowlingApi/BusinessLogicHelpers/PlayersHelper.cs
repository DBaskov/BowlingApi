using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bowling.Api.DTOs;
using BowlingApi.Common.CustomExceptions;
using BowlingApi.Repositories.Models;
using BowlingApi.Repositories.Models.HelperModels;
using BowlingApi.Repository;

namespace BowlingApi.BusinessLogicHelpers
{
    public class PlayersHelper : IPlayersHelper
    {
        enum SpecialScores { Spare = 10, Strike = 11}; 

        public readonly IPlayersDataRepository _playersDataService;
        public PlayersHelper(IPlayersDataRepository playersDataService)
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
            var frame = new Frame() { ScoreCells = new List<int>() { numPins } };

            if (score.ResultList.Count == 0) //perhaps move this to models instatiation
            {
                score.RunningTotalList.Add(score.TotalScore);
                UpdateFrameScore(score.ResultList, 1, 1, numPins);
            }
            else
            {
                frame = score.ResultList[score.ResultList.Count - 1];
                var cellNum = frame.ScoreCells.Count;
                if (cellNum > 0) //optimize this later
                {

                    if (cellNum == 1 && !frame.IsStrike || score.ResultList.Count == 10) //also logic for 10
                    {
                        cellNum += 1;
                        
                        score.RunningTotalList[score.ResultList.Count - 1] = score.TotalScore;
                        UpdateFrameScore(score.ResultList, score.ResultList.Count, cellNum, numPins);
                    }
                    else
                    {
                        cellNum = 1;
                        score.RunningTotalList.Add(score.TotalScore);

                        UpdateFrameScore(score.ResultList, score.ResultList.Count, cellNum, numPins);
                    }
                }               

                HandleStrikesAndSpares(score, score.ResultList.Count, cellNum, numPins);
            }
            
            await _playersDataService.UpdatePlayerData(score); 

            return score; 
        }

        public void UpdateBonusFrame(Frame bonusFrame, int cellNum, int numPins)
        {

        }

        public void UpdateFrameScore(List<Frame> resultList, int frameNum, int cellNum, int numPins)
        {
            Frame frame = null;
            if (cellNum == 1)
            { //watch out for spares
                frame = new Frame() { ScoreCells = new List<int>() { numPins } };
            }
            else
            {
                frame = resultList[frameNum - 1];
            }

            if (numPins == 10)
            {
                if (cellNum == 1 || frameNum == 10)
                {
                    frame.IsStrike = true;
                }
            }

            var oneShotBackResult = GetScoreOneShotBackAndFrameNum(resultList, frameNum, cellNum);
            if (numPins + oneShotBackResult.Item1 == 10)
            {
                frame.IsSpare = true;
            }

            if (cellNum >= 2)
            {
                frame.ScoreCells.Add(numPins);
            }
            else //add new frame
            {
                resultList.Add(frame);
            }

        }

        public async Task<PlayerGameData> GetPlayerGameData(Guid playerId)
        {

            var result = await _playersDataService.GetPlayerData(playerId.ToString());
            return result;           
        }

        public async Task<bool> ReplacePlayerGameData(PlayerGameDataIn playerGameDataIn)
        {
            var playerGameData = new PlayerGameData
            {
                PlayerId = playerGameDataIn.PlayerId,
                PlayerName = playerGameDataIn.PlayerName,
                TotalScore = playerGameDataIn.TotalScore,
                ResultList = playerGameDataIn.ResultList,
                RunningTotalList = playerGameDataIn.RunningTotalList
            };

            var success = await _playersDataService.ReplacePlayerData(playerGameData);

            if (!success)
            {
                throw new MongoOperationFailException("Mongo 'AddPlayers' operation failed. ");
            }

            return success;
        }

        public Tuple<bool, int> FoundStrikeTwoShotsBackAndFrameNum(List<Frame> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum < 2)
            {
                return new Tuple<bool, int>(false, -1);
            }

            var frameNumResult = curFrameNum - 2; //zero indexed, frame before current
            if (curCellNum == 1)
            {
                if (frameNumResult >= 0 && resultList[frameNumResult].IsStrike)
                {
                    frameNumResult--;
                    if (frameNumResult >= 0 && resultList[frameNumResult].IsStrike)
                    {
                        return new Tuple<bool, int>(true, frameNumResult + 1);
                    }
                }

                return new Tuple<bool, int>(false, -1);
            }
            else
            {
                if (resultList[frameNumResult].IsStrike) // if 10 is in first cell, we have a strike [10][-1] [4], need to go back one more
                {
                    return new Tuple<bool, int>(true, frameNumResult + 1);
                }
                return new Tuple<bool, int>(false, -1);
            }
        }

        public Tuple<bool, int> FoundSpareOneShotBackAndFrameNum(List<Frame> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum < 2)
            {
                return new Tuple<bool, int>(false, -1);
            }

            var frameNumResultIdx = curFrameNum - 1;
            if (curCellNum == 1)
            {
                frameNumResultIdx -= 1;

                return new Tuple<bool, int>(resultList[frameNumResultIdx].IsSpare, curFrameNum - 1);
            }
            else if(curFrameNum == 10) //handle bonus frame
            {
                if(curFrameNum == 2)
                {
                    return new Tuple<bool, int>(false, curFrameNum);
                }
                else if(resultList[frameNumResultIdx].ScoreCells[0] + resultList[frameNumResultIdx].ScoreCells[1] == 10) //curFrameNum == 3
                {
                    return new Tuple<bool, int>(true, curFrameNum);
                }
            }

            return new Tuple<bool, int>(resultList[frameNumResultIdx].IsSpare, curFrameNum); //do we need this?
        }

        public Tuple<int, int> GetScoreOneShotBackAndFrameNum(List<Frame> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum == 1 && curCellNum == 1)
            {
                return new Tuple<int, int>(0, -1);
            }

            var frameNumResultIdx = curFrameNum - 1;
            if (curCellNum == 1)
            {
                frameNumResultIdx -= 1;
                if (resultList[frameNumResultIdx].IsStrike)
                {
                    return new Tuple<int, int>(10, frameNumResultIdx + 1);
                }

                return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[1], frameNumResultIdx + 1);
            }
            else
            {
                if(curFrameNum == 10)
                {
                    if (curCellNum == 1 || curCellNum == 2)
                    {
                        frameNumResultIdx -= 1;
                        if (resultList[frameNumResultIdx].IsStrike)
                        {
                            return new Tuple<int, int>(10, frameNumResultIdx + 1);
                        }
                    }
                    else
                    {
                        return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[1], frameNumResultIdx + 1);
                    }
                }
                
                return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[0], frameNumResultIdx + 1);
            }
        }

        public void RecalculateRunningTotal(List<int> runningTotal, int newScore, int frameNum)
        {
            if (runningTotal.Count < 2)
            {
                return;
            }
            //todo additional logic to check if params are valid
            var frameIdx = frameNum - 1;
            var scoreDifference = newScore - runningTotal[frameIdx];
            runningTotal[frameIdx] = newScore;

            for (var i = frameIdx + 1; i < runningTotal.Count; i++)
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

                    var newScore = shotBeforeResult.Item1 + numPins + 10; //spares are handled

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
                var spareFoundResult = FoundSpareOneShotBackAndFrameNum(resultList, frameNum, cellNum);

                if (spareFoundResult.Item1) //it's a spare
                {
                    var spareFrameNewScore = playerScore.RunningTotalList[spareFoundResult.Item2 - 1] + numPins; //expect strikes and spare to have score of 10
                    RecalculateRunningTotal(playerScore.RunningTotalList, spareFrameNewScore, spareFoundResult.Item2);
                    playerScore.TotalScore = playerScore.RunningTotalList[frameNum - 1];
                    return true;
                }
            }

            return false;
        }

        private bool HandleStrikesAndSpares(PlayerGameData playerScore, int frameNum, int cellNum, int numPins)
        {

            return HandleStrike(playerScore, frameNum, cellNum, numPins)
                || HandleSpare(playerScore, frameNum, cellNum, numPins); //when going back if you already got spare and count another one
        }
    }
}
