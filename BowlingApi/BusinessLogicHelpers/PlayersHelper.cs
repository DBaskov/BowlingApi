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

        public async Task<PlayerGameData> UpdateScore(Guid playerId, int numPins)
        {
                           
            var score = await GetPlayerGameData(playerId);

            ComputeNewScore(score, numPins);

            await _playersDataService.UpdatePlayerData(score); 
            return score; 
        }        

        public void ComputeNewScore(PlayerGameData playerScore, int numPins)
        {
            Frame frame = null;
            var cellNum = 0;
            var frameNum = 1;
        
            if (playerScore.ResultList.Count > 0)
            {
                frame = playerScore.ResultList[playerScore.ResultList.Count - 1];
                frameNum = playerScore.ResultList.Count;
                cellNum = frame.ScoreCells.Count;
            }

            if (!(playerScore.ResultList.Count == 10 && cellNum == 2))
            {
                playerScore.TotalScore += numPins;
            }

            cellNum = UpdateRunningTotalAndReturnNewCellNum(playerScore.RunningTotalList, cellNum, playerScore, frame);
            UpdateResultList(playerScore.ResultList, frameNum, cellNum, numPins);
            frameNum = playerScore.ResultList.Count;
            HandleStrikesAndSpares(playerScore, frameNum, cellNum, numPins);
        }

        public int UpdateRunningTotalAndReturnNewCellNum(List<int> runningTotal, int cellNum, PlayerGameData playerGameData, Frame curFrame)
        {
            //we expected Frame to not be null if cellNum = 1;
            if (cellNum == 1 && !FrameHasStrike(curFrame) || playerGameData.ResultList.Count == 10) //also logic for 10
            {                
                runningTotal[playerGameData.ResultList.Count - 1] = playerGameData.TotalScore;
                return cellNum += 1;
            }
            else
            {              
                runningTotal.Add(playerGameData.TotalScore);
                return 1;
            }
        }

        public void UpdateResultList(List<Frame> resultList, int frameNum, int cellNum, int numPins)
        {
            Frame frame;
            if (cellNum == 1)
            { //watch out for spares
                frame = new Frame() { ScoreCells = new List<int>() { numPins } };
            }
            else
            {
                frame = resultList[frameNum - 1];
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

        public Tuple<bool, int> FoundStrikeTwoShotsBackAndFrameNum(List<Frame> resultList, int curFrameNum, int curCellNum)
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

        public bool FrameHasStrike(Frame frame)
        {
            if (frame == null || frame.ScoreCells.Count < 1)
            {
                return false;
            }

            return frame.ScoreCells[0] == 10;
        }

        public bool FrameHasSpare(Frame frame)
        {
            if(frame == null || frame.ScoreCells.Count < 2)
            {
                return false;
            }

            return frame.ScoreCells[0] + frame.ScoreCells[1] == 10;
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
                return new Tuple<bool, int>(FrameHasSpare(resultList[frameNumResultIdx]), curFrameNum - 1);
            }
            else if(curFrameNum == 10) //handle bonus frame
            {               
                if(FrameHasSpare(resultList[frameNumResultIdx])) //curFrameNum == 3
                {
                    return new Tuple<bool, int>(true, curFrameNum);
                }
            }

            return new Tuple<bool, int>(FrameHasSpare(resultList[frameNumResultIdx]), curFrameNum); //do we need this?
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
                if (FrameHasStrike(resultList[frameNumResultIdx]))
                {
                    return new Tuple<int, int>(10, frameNumResultIdx + 1);
                }

                return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[1], frameNumResultIdx + 1);
            }
            else
            {
                if(curFrameNum == 10)
                {
                    if (curCellNum == 2)
                    {                        
                        return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[0], frameNumResultIdx + 1);
                    }
                    else //cell 3 in bonus frame
                    {
                        return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[1], frameNumResultIdx + 1);
                    }
                }
                
                return new Tuple<int, int>(resultList[frameNumResultIdx].ScoreCells[0], frameNumResultIdx + 1);
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

        public bool HandleSpare(PlayerGameData playerScore, int frameNum, int cellNum, int numPins)
        {
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

        private bool HandleStrikesAndSpares(PlayerGameData playerScore, int frameNum, int cellNum, int numPins)
        {

            return HandleStrike(playerScore, frameNum, cellNum, numPins)
                || HandleSpare(playerScore, frameNum, cellNum, numPins);
        }
    }
}
