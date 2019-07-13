using BowlingApi.BusinessLogicHelpers;
using BowlingApi.Services;
using BowlingApi.Services.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bowling.Api.Tests
{
    [TestClass] //todo flip expected and actual values
    public class PlayersHelperTests
    {
        private Mock<IPlayersDataService> mockPlayersDataService;
        private PlayersHelper playersHelper;
        private readonly Guid PLAYER_ID = Guid.NewGuid();
        
        [TestInitialize]
        public void Initialize()
        {
            mockPlayersDataService = new Mock<IPlayersDataService>(MockBehavior.Strict);

            //mockPlayersDataService.Setup(x => x.AddPlayer(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));
           // mockPlayersDataService.Setup(x => x.AddPlayers(It.IsAny<List<PlayerGameData>>())).Returns(Task.FromResult(true));

            playersHelper = new PlayersHelper(mockPlayersDataService.Object);
        }

        [TestMethod]
        public async Task InstiateAndInsertPlayerGameDataValidTestMethod()
        {
            mockPlayersDataService.Setup(x => x.DeletePlayer(It.IsAny<string>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);

            var playerDataResult = await helper.DeletePlayerGameData("John");

            Assert.IsTrue(playerDataResult);
            //Assert.AreEqual("John", playerDataResult.PlayerName); 
            //Assert.IsTrue(Guid.TryParse(playerDataResult.PlayerId, out var guidResult));
        }

        [TestMethod] //todo check PLAYER_ID matches on all of them
        public async Task UpdateScoreSingleValueTest()
        {
            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(new PlayerGameData()));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 4;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, numPins);
            Assert.AreEqual(result.ResultList[0][0], numPins);
        }

        private static PlayerGameData CreatePlayerData(int totalScore, List<int> runningTotal, List<int> frame)
        {
            return new PlayerGameData
            {
                TotalScore = totalScore,
                RunningTotalList = runningTotal,
                ResultList = new List<List<int>>() { frame }
            };
        }

        [TestMethod]
        public async Task UpdateScoreSecondValueTest()
        {

            var playerScore = new PlayerGameData();
            var oldTotalScore = 4;
            var oldRunningTotalList = new List<int> { 4 };
            var oldFrame = new List<int>() { 4 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            //playerScore.TotalScore = oldTotalScore;
            //playerScore.RunningTotalList = oldRunningTotalList;
            //playerScore.ResultList = oldResultList;

            //var playerScoreOld = playerScore;

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 5;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, oldPlayerScore.TotalScore + numPins);
            Assert.AreEqual(result.RunningTotalList[0], oldPlayerScore.TotalScore + numPins);
            Assert.AreEqual(result.ResultList[0][0], oldPlayerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], numPins);           
        }

        [TestMethod]
        public async Task UpdateScoreThirdValueTest()
        {                       
            var playerScore = new PlayerGameData();
            var oldTotalScore = 9;
            var oldRunningTotalList = new List<int> { 9 };
            var oldFrame = new List<int>() { 4, 5 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, oldPlayerScore.TotalScore + numPins);
            Assert.AreEqual(result.RunningTotalList[1], oldPlayerScore.TotalScore + numPins);
            Assert.AreEqual(result.ResultList[0][0], oldPlayerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], oldPlayerScore.ResultList[0][1]);
            Assert.AreEqual(result.ResultList[1][0], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreMissFirstShotTest()
        {
            var playerScore = new PlayerGameData();            

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 0);
            Assert.AreEqual(result.RunningTotalList[0], 0);
            Assert.AreEqual(result.ResultList[0][0], 0);            
        }

        [TestMethod]
        public async Task UpdateScoreMissBothFirstShotsTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 0;
            var oldRunningTotalList = new List<int> { 0 };
            var oldFrame = new List<int>() { 0 };

            //var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 0);
            Assert.AreEqual(result.RunningTotalList[0], 0);
            Assert.AreEqual(result.ResultList[0][0], 0);
            Assert.AreEqual(result.ResultList[0][1], 0);
        }

        [TestMethod]
        public async Task UpdateScoreMissSecondShotTest()
        {            
            var playerScore = new PlayerGameData();
            var oldTotalScore = 4;
            var oldRunningTotalList = new List<int> { 4 };
            var oldFrame = new List<int>() { 4 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, oldPlayerScore.TotalScore);
            Assert.AreEqual(result.RunningTotalList[0], oldPlayerScore.RunningTotalList[0]);
            Assert.AreEqual(result.ResultList[0][0], oldPlayerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreSpareTest()
        {           
            var playerScore = new PlayerGameData();
            var oldTotalScore = 4;
            var oldRunningTotalList = new List<int> { 4 };
            var oldFrame = new List<int>() { 4 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10);
            Assert.AreEqual(result.RunningTotalList[0], oldPlayerScore.RunningTotalList[0]);
            Assert.AreEqual(result.ResultList[0][0], oldPlayerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreShotAfterSpareTest()
        {            
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            var oldFrame = new List<int>() { 4, 10 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10 + numPins + numPins);
            Assert.AreEqual(result.RunningTotalList[0], 10 + numPins);
            Assert.AreEqual(result.RunningTotalList[1], numPins);
            Assert.AreEqual(result.ResultList[0][0], oldPlayerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], oldPlayerScore.ResultList[0][1]);
            Assert.AreEqual(result.ResultList[1][0], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeAfterSpareTest()
        {            
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            var oldFrame = new List<int>() { 4, 10 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10 + numPins + numPins);
            Assert.AreEqual(result.RunningTotalList[0], 10 + numPins);
            Assert.AreEqual(result.RunningTotalList[1], 10 + numPins + numPins);
            Assert.AreEqual(result.ResultList[0][0], oldPlayerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], oldPlayerScore.ResultList[0][1]);
            Assert.AreEqual(result.ResultList[1][0], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreSpareAfterMissTest()
        {
            var playerScore = new PlayerGameData();     //todo need to tell decide on how to instatiate ResultList or specify the frame num via Api       

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10 + numPins + numPins);
            Assert.AreEqual(result.RunningTotalList[0], 10 + numPins);
            Assert.AreEqual(result.RunningTotalList[1], numPins);
            Assert.AreEqual(result.ResultList[0][0], playerScore.ResultList[0][0]);
            Assert.AreEqual(result.ResultList[0][1], playerScore.ResultList[0][1]);
            Assert.AreEqual(result.ResultList[1][0], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreMissAfterSpareTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreShotAfterStrikeTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSecondShotAfterStrikeTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreThirdShotAfterStrikeTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSecondInRowStrikeTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreThirdInRowStrikeTest()
        {

        }        

        [TestMethod]
        public async Task UpdateScoreStrikeSpareTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeStrikeRegularShotTest()
        {

        }                

        [TestMethod]
        public async Task UpdateScoreStrikeMissRegularShotTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeRegularShotMiss()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeMissMiss()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeMissSpare()
        {

        }

        [TestMethod]
        public async Task UpdateScoreNextFrame()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSecondShotInBonusFrameTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreThirdShotBonusFrameTest()
        {

        }       

        [TestMethod]
        public async Task UpdateScoreTripleStrikeBonusFrameTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreDoubleMissBonusFrameTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeSpareBonusFrame()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSpareStrikeBonusFrame()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSpareRegularShotBonus()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeStrikeRegularBonusFrame()
        {

        }

        [TestMethod]
        public async Task UpdateScoreAllMissTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreAllStrikesTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreAllSparesTest()
        {

        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumOneScoreTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 4 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual(result.Item2, -1);
            Assert.AreEqual(result.Item1, 0);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumTwoScoreTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 4, 4 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 2);
            
            Assert.AreEqual(4, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumThreeScoreTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 4, 4 }, new List<int>() { 5 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(4, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumSpareFristFrameTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 5, 10 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual( 0, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumSpareSecondFrameTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 4, 4 }, new List<int>() { 5, 10 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 2);

            Assert.AreEqual(4, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }


        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumStrikeFirstFrame()
        {
            var resultList = new List<List<int>>() { new List<int>() { 10, -1 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual(0 , result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumStrikeSecondFrameTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 4, 4 }, new List<int>() { 10, -1 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(4, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumStrikeSecondFrameSecondCellTest()
        {
            var resultList = new List<List<int>>() { new List<int>() { 4, 4 }, new List<int>() { 10, -1 } };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 2);

            Assert.AreEqual(0 , result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackFirstShotTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackSecondShotTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackThirdShotTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackFourthShotTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackSpareShotBeforeTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackStrikeShotBeforeTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackThirdShotSpareFirstFrameTest()
        {

        }

        [TestMethod]
        public async Task GetScoreTwoShotsBackThirdShotStrikeFirstFrameTest()
        {

        }
        
    }
}
