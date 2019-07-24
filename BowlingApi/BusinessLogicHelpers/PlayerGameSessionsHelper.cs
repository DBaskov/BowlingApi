using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bowling.Api.DTOs;
using BowlingApi.Common.CustomExceptions;
using BowlingApi.Repositories.Models;
using BowlingApi.Repository;

namespace BowlingApi.BusinessLogicHelpers
{
    public class PlayerGameSessionsHelper : IPlayerGameSessionsHelper
    {
        enum SpecialScores { Spare = 10, Strike = 11}; 

        public readonly IPlayerGameSessionsRepository _playersDataService;
        public PlayerGameSessionsHelper(IPlayerGameSessionsRepository playersDataService)
        {
            _playersDataService = playersDataService;
        }

        public async Task<List<PlayerGameSession>> ChangeFrameScore(string playerId, int frameNumber, int newScore)
        {
            throw new NotImplementedException();
        }
        
        public async Task<PlayerGameSession> InstiateAndInsertPlayerGameData(string playerName)
        {
            var playerGameData = new PlayerGameSession
            {
                PlayerGameSessionId = Guid.NewGuid().ToString(),
                PlayerName = playerName
            };

            var success = await _playersDataService.Add(playerGameData);
            if (!success)
            {
                throw new MongoOperationFailException("Mongo 'AddPlayers' operation failed. ");
            }

            return playerGameData;
        } 

        public async Task<bool> DeletePlayerGameData(string playerId)
        {
            var result = await _playersDataService.Delete(playerId);

            return result;
        }

        public async Task<bool> DeleteBulkPlayerGameData(List<string> playerIds)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PlayerGameSession>> InstatiateBulkPlayerGameData(List<string> playerNames)
        {
            var playersList = new List<PlayerGameSession>();

            var matchId = Guid.NewGuid().ToString();
            foreach (var playerName in playerNames)
            {
                playersList.Add(
                    new PlayerGameSession
                    {
                        PlayerGameSessionId = Guid.NewGuid().ToString(),
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

        public async Task<PlayerGameSession> GetPlayerGameData(Guid playerId)
        {
            var result = await _playersDataService.Get(playerId.ToString());
            return result;
        }

        public async Task<bool> ReplacePlayerGameData(PlayerGameDataIn playerGameDataIn)
        {
            var playerGameData = new PlayerGameSession
            {
                PlayerGameSessionId = playerGameDataIn.PlayerId,
                PlayerName = playerGameDataIn.PlayerName,
                TotalScore = playerGameDataIn.TotalScore,
                ResultList = playerGameDataIn.ResultList,
                RunningTotalList = playerGameDataIn.RunningTotalList
            };

            var success = await _playersDataService.Replace(playerGameData);

            if (!success)
            {
                throw new MongoOperationFailException("Mongo 'AddPlayers' operation failed. ");
            }

            return success;
        }        

        public async Task<PlayerGameSession> UpdateScore(Guid playerId, int numPins)
        {
                           
            var score = await GetPlayerGameData(playerId);

            ComputeNewScore(score, numPins);

            await _playersDataService.Update(score); 
            return score; 
        }        

        public void ComputeNewScore(PlayerGameSession playerScore, int numPins)
        {
            UpdateTotal(playerScore, numPins);
            UpdateRunningTotal(playerScore.RunningTotalList, playerScore);
            UpdateResultList(playerScore.ResultList, numPins);           
            HandleStrike(playerScore, numPins);
            HandleSpare(playerScore, numPins);
        }

        public void UpdateTotal(PlayerGameSession playerScore, int numPins)
        {
            var currentFrameAndCellNum = GetCurrentFrameAndCellNum(playerScore.ResultList);

            if (!(currentFrameAndCellNum.Item1 == 10 && currentFrameAndCellNum.Item2 == 2))
            {
                playerScore.TotalScore += numPins;
            }
        }

        public void UpdateRunningTotal(List<int> runningTotal, PlayerGameSession playerGameData)
        {
            var currentFrameAndCellNum =  GetCurrentFrameAndCellNum(playerGameData.ResultList);
            var cellNum = currentFrameAndCellNum.Item2;

            List<int> curFrame = null;
            if (playerGameData.ResultList.Count > 0)
            {
                curFrame = playerGameData.ResultList[currentFrameAndCellNum.Item1 - 1];
            }

            //we expected Frame to not be null if cellNum = 1;
            if (cellNum == 1 && !FrameHasStrike(curFrame) || playerGameData.ResultList.Count == 10) //also logic for 10
            {                
                runningTotal[playerGameData.ResultList.Count - 1] = playerGameData.TotalScore;
            }
            else
            {              
                runningTotal.Add(playerGameData.TotalScore);
            }
        }

        public void UpdateResultList(List<List<int>> resultList, int numPins)
        {
            List<int> frame;
            var frameAndCellNum = GetNewFrameAndCellNum(resultList);
            var frameNum = frameAndCellNum.Item1;
            var cellNum = frameAndCellNum.Item2;

            if (cellNum == 1)
            { //watch out for spares
                frame = new List<int>() { numPins };
                resultList.Add(frame);
            }
            else
            {
                frame = resultList[frameNum - 1];
                frame.Add(numPins);
            }                        
        }

        public Tuple<bool, int> FoundStrikeTwoShotsBackAndFrameNum(List<List<int>> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum < 2)
            {
                return new Tuple<bool, int>(false, -1);
            }

            var frameNumResultIdx = curFrameNum - 1; //zero indexed, frame before current
            if (curCellNum != 1) 
            {
                if (!(curFrameNum == 10 && curCellNum == 3)) //third shot in bonus frame, looks within same frame two shots back
                {
                    frameNumResultIdx -= 1;
                }

                if (FrameHasStrike(resultList[frameNumResultIdx])) // if 10 is in first cell, we have a strike [10][-1] [4], need to go back one more
                {
                    return new Tuple<bool, int>(true, frameNumResultIdx + 1);
                }
                return new Tuple<bool, int>(false, -1);                
            }
            else
            {
                frameNumResultIdx--;
                if (frameNumResultIdx >= 0 && FrameHasStrike(resultList[frameNumResultIdx])) //found strike one frame back
                {
                    frameNumResultIdx--; //skip over frame if we got strike in the back
                    if (frameNumResultIdx >= 0 && FrameHasStrike(resultList[frameNumResultIdx])) //found strike second frame back
                    {
                        return new Tuple<bool, int>(true, frameNumResultIdx + 1);
                    }
                }
                return new Tuple<bool, int>(false, -1);
            }
        }

        public bool FrameHasStrike(List<int> frame)
        {
            if (frame == null || frame.Count < 1)
            {
                return false;
            }

            return frame[0] == 10;
        }

        public bool FrameHasSpare(List<int> frame)
        {
            if(frame == null || frame.Count < 2)
            {
                return false;
            }

            if(frame[0] == 10)
            {
                return false;
            }

            return frame[0] + frame[1] == 10;
        }

        public Tuple<int, int> GetCurrentFrameAndCellNum(List<List<int>> resultList)
        {
            if (resultList.Count == 0)
            {
                return new Tuple<int, int>(0, 0);
            }

            var frame = resultList[resultList.Count - 1];

            return new Tuple<int, int>(resultList.Count, frame.Count);
        }

        public Tuple<int, int> GetNewFrameAndCellNum(List<List<int>> resultList)
        {
            var currentFrameAndCellNum = GetCurrentFrameAndCellNum(resultList);

            if (currentFrameAndCellNum.Item1 == 0)
            {
                return new Tuple<int, int>(1, 1);
            }

            if (FrameHasStrike(resultList[currentFrameAndCellNum.Item1 - 1]) && currentFrameAndCellNum.Item1 < 10)
            {
                return new Tuple<int, int>(currentFrameAndCellNum.Item1 + 1, 1);
            }
            else if (currentFrameAndCellNum.Item2 == 2 && currentFrameAndCellNum.Item1 < 10)
            {
                return new Tuple<int, int>(currentFrameAndCellNum.Item1 + 1, 1);
            }
            else
            {
                return new Tuple<int, int>(currentFrameAndCellNum.Item1, currentFrameAndCellNum.Item2 + 1);
            }

        }

        public Tuple<bool, int> FoundSpareOneShotBackAndFrameNum(List<List<int>> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum < 2)
            {
                return new Tuple<bool, int>(false, -1);
            }

            var frameNumResultIdx = curFrameNum - 1;
            if (curCellNum == 1)
            {
                frameNumResultIdx -= 1;               
                return new Tuple<bool, int>(FrameHasSpare(resultList[frameNumResultIdx]), curFrameNum - 1);
            }
            else if(curFrameNum == 10 && curCellNum == 3) //handle bonus frame
            {               
                if(FrameHasSpare(resultList[frameNumResultIdx])) //curFrameNum == 3
                {
                    return new Tuple<bool, int>(true, curFrameNum);
                }
                return new Tuple<bool, int>(false, -1);
            }
            else //cell num is 2, there won't be any spares
            {
                return new Tuple<bool, int>(false, -1);
            }
        }

        public Tuple<int, int> GetScoreOneShotBackAndFrameNum(List<List<int>> resultList, int curFrameNum, int curCellNum)
        {
            if (curFrameNum == 1 && curCellNum == 1)
            {
                return new Tuple<int, int>(0, -1);
            }

            var frameNumResultIdx = curFrameNum - 1;
            if (curCellNum == 1)
            {
                frameNumResultIdx -= 1;
                if (FrameHasStrike(resultList[frameNumResultIdx]))
                {
                    return new Tuple<int, int>(10, frameNumResultIdx + 1);
                }

                return new Tuple<int, int>(resultList[frameNumResultIdx][1], frameNumResultIdx + 1);
            }
            else
            {
                if(curFrameNum == 10)
                {
                    if (curCellNum == 2)
                    {                        
                        return new Tuple<int, int>(resultList[frameNumResultIdx][0], frameNumResultIdx + 1);
                    }
                    else //cell 3 in bonus frame
                    {
                        return new Tuple<int, int>(resultList[frameNumResultIdx][1], frameNumResultIdx + 1);
                    }
                }
                
                return new Tuple<int, int>(resultList[frameNumResultIdx][0], frameNumResultIdx + 1);
            }
        }

        public void RecalculateRunningTotal(List<int> runningTotal, int newScore, int oldScore, int frameNum)
        {
            if (runningTotal.Count < 2)
            {
                return;
            }

            var frameIdx = frameNum - 1;
            var scoreDifference = newScore - oldScore;
            runningTotal[frameIdx] = runningTotal[frameIdx] + scoreDifference;

            for (var i = frameIdx + 1; i < runningTotal.Count; i++)
            {
                runningTotal[i] += scoreDifference;
            }
        }

        public bool HandleStrike(PlayerGameSession playerScore, int numPins)
        {
            var frameAndCellNum = GetCurrentFrameAndCellNum(playerScore.ResultList);
            var frameNum = frameAndCellNum.Item1;
            var cellNum = frameAndCellNum.Item2;

            var resultList = playerScore.ResultList;
            if (frameNum > 1)
            {
                var foundStrikeAndFrame = FoundStrikeTwoShotsBackAndFrameNum(resultList, frameNum, cellNum);

                if (foundStrikeAndFrame.Item1)
                {
                    var shotBeforeResult = GetScoreOneShotBackAndFrameNum(resultList, frameNum, cellNum);

                    var newScore = shotBeforeResult.Item1 + numPins + 10; //spares are handled
                    var oldScore = 10;
                    if(frameNum == 10 && cellNum == 3) //bonus frame can have more cells
                    {
                        oldScore = shotBeforeResult.Item1 + 10;
                    }

                    RecalculateRunningTotal(playerScore.RunningTotalList, newScore, oldScore, foundStrikeAndFrame.Item2);
                    playerScore.TotalScore = playerScore.RunningTotalList[frameNum - 1];
                    return true;
                }
            }

            return false;
        }

        public bool HandleSpare(PlayerGameSession playerScore, int numPins)
        {
            var frameAndCellNum = GetCurrentFrameAndCellNum(playerScore.ResultList);
            var frameNum = frameAndCellNum.Item1;
            var cellNum = frameAndCellNum.Item2;

            var resultList = playerScore.ResultList;
            if (frameNum > 1)
            {
                var spareFoundResult = FoundSpareOneShotBackAndFrameNum(resultList, frameNum, cellNum);

                if (spareFoundResult.Item1) //it's a spare
                {
                    var spareFrameNewScore = 10 + numPins; //expect strikes and spare to have score of 10
                    RecalculateRunningTotal(playerScore.RunningTotalList, spareFrameNewScore, 10,spareFoundResult.Item2);
                    playerScore.TotalScore = playerScore.RunningTotalList[frameNum - 1];
                    return true;
                }
            }

            return false;
        }        
    }
}
