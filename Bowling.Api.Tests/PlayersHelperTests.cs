using BowlingApi.BusinessLogicHelpers;
using BowlingApi.Repositories.Models;
using BowlingApi.Repositories.Models.HelperModels;
using BowlingApi.Repository;
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
        private Mock<IPlayersDataRepository> mockPlayersDataService;
        private PlayersHelper playersHelper;
        private readonly Guid PLAYER_ID = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            mockPlayersDataService = new Mock<IPlayersDataRepository>(MockBehavior.Strict);

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
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], numPins);
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

            Assert.AreEqual(oldPlayerScore.TotalScore + numPins, result.TotalScore);
            Assert.AreEqual(oldPlayerScore.TotalScore + numPins, result.RunningTotalList[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(numPins, result.ResultList[0].ScoreCells[1]);
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

            Assert.AreEqual(oldPlayerScore.TotalScore + numPins, result.TotalScore);
            Assert.AreEqual(oldPlayerScore.TotalScore + numPins, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[1], result.ResultList[0].ScoreCells[1]);
            Assert.AreEqual(numPins, result.ResultList[1].ScoreCells[0]);
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
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], 0);
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
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], 0);
            Assert.AreEqual(result.ResultList[0].ScoreCells[1], 0);
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

            Assert.AreEqual(oldPlayerScore.TotalScore, result.TotalScore);
            Assert.AreEqual(oldPlayerScore.RunningTotalList[0], result.RunningTotalList[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(numPins, result.ResultList[0].ScoreCells[1]);
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
            Assert.AreEqual(oldPlayerScore.RunningTotalList[0], result.RunningTotalList[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(numPins, result.ResultList[0].ScoreCells[1]);
        }
         
        [TestMethod]
        public async Task UpdateScoreShotAfterSpareTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            var oldFrame = new List<int>() { 4, 6 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);            

            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(10 + numPins + numPins, result.TotalScore);
            Assert.AreEqual(10 + numPins, result.RunningTotalList[0]);
            Assert.AreEqual(result.RunningTotalList[0] + numPins, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[1], result.ResultList[0].ScoreCells[1]);

            Assert.AreEqual(numPins, result.ResultList[1].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeAfterSpareTest() //same issue
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            var oldFrame = new List<int>() { 4, 6 };

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
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], oldPlayerScore.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[0].ScoreCells[1], oldPlayerScore.ResultList[0].ScoreCells[1]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[0], numPins);

        }

        [TestMethod]
        public async Task UpdateScoreSpareAfterMissTest()
        {
            var playerScore = new PlayerGameData();     //todo need to tell decide on how to instatiate ResultList or specify the frame num via Api 
            var oldTotalScore = 0;
            var oldRunningTotalList = new List<int> { 0 };
            var oldFrame = new List<int>() { 0 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10);
            Assert.AreEqual(result.RunningTotalList[0], 10);
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], playerScore.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[0].ScoreCells[1], numPins);
        }

        [TestMethod]
        public async Task UpdateScoreMissAfterSpareTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            var oldFrame = new List<int>() { 4, 6 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
  

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10);
            Assert.AreEqual(result.RunningTotalList[0], 10);
            Assert.AreEqual(result.RunningTotalList[1], 10);
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], oldPlayerScore.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[0].ScoreCells[1], oldPlayerScore.ResultList[0].ScoreCells[1]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[0], numPins);

        }
        
        [TestMethod]
        public async Task UpdateScoreStrikeTest()
        {
            var playerScore = new PlayerGameData();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 10);
            Assert.AreEqual(result.RunningTotalList[0], 10);
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], 10);
        }

        [TestMethod]
        public async Task UpdateScoreShotAfterStrikeTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            var oldFrame = new List<int>() { 10 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            ;

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 4;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, 14);
            Assert.AreEqual(result.RunningTotalList[0], 10);
            Assert.AreEqual(result.RunningTotalList[1], 14);
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], oldPlayerScore.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[0], numPins);
            
        }

        [TestMethod]
        public async Task UpdateScoreSecondShotAfterStrikeTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 14;
            var oldRunningTotalList = new List<int> { 10, 14 };
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 5;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, (10 + 4 + numPins) + 4 + numPins);
            Assert.AreEqual(result.RunningTotalList[0], 10 + 4 + numPins);
            Assert.AreEqual(result.RunningTotalList[1], result.TotalScore);
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], oldPlayerScore.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[0], oldPlayerScore.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[1], numPins);
            
        }

        [TestMethod]
        public async Task UpdateScoreThirdShotAfterStrikeTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 28;
            var oldRunningTotalList = new List<int> { 19, 28 };           
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4, 5 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 3;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(result.TotalScore, oldTotalScore + numPins);
            Assert.AreEqual(result.RunningTotalList[0], 19);
            Assert.AreEqual(result.RunningTotalList[1], 28);
            Assert.AreEqual(result.RunningTotalList[2], 31);
            Assert.AreEqual(result.ResultList[0].ScoreCells[0], oldPlayerScore.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[0], oldPlayerScore.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(result.ResultList[1].ScoreCells[1], oldPlayerScore.ResultList[1].ScoreCells[1]);
            Assert.AreEqual(result.ResultList[2].ScoreCells[0], numPins);
            
        }

        [TestMethod] //failing because logic is going off from new -1 created not the 10
        public async Task UpdateScoreSecondInRowStrikeTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10 };
            //var oldFrames = new List<List<int>>() {
           //     new List<int>() { 10, -1 }
          //  };

            var oldFrame = new List<int>() { 10 };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrame);
            ;

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(20, result.TotalScore);
            Assert.AreEqual(10, result.RunningTotalList[0]);
            Assert.AreEqual(20, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(10, result.ResultList[1].ScoreCells[0]);
            
            
        }
        
        [TestMethod]
        public async Task UpdateScoreThirdInRowStrikeTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeSpareTest() //look more into this error see how unique it is
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 14;
            var oldRunningTotalList = new List<int> { 10, 14 };            
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(10 + 10 + 10, result.TotalScore);
            Assert.AreEqual(20, result.RunningTotalList[0]);
            Assert.AreEqual(30, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(6, result.ResultList[1].ScoreCells[1]);
            
        }
        
        [TestMethod]
        public async Task UpdateScoreStrikeSpareRegularShotTest() //look more into this error see how unique it is
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 30;
            var oldRunningTotalList = new List<int> { 20, 30 };            
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4, 6 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(20 + 16 + 6, result.TotalScore);
            Assert.AreEqual(20, result.RunningTotalList[0]);
            Assert.AreEqual(36, result.RunningTotalList[1]);
            Assert.AreEqual(42, result.RunningTotalList[2]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[1], result.ResultList[1].ScoreCells[1]);
            Assert.AreEqual(numPins, result.ResultList[2].ScoreCells[0]);
            
        }

        [TestMethod]
        public async Task UpdateScoreStrikeStrikeRegularShotTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 20;
            var oldRunningTotalList = new List<int> { 10, 20 };            
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(26 + 10 + 6, result.TotalScore);
            Assert.AreEqual(26, result.RunningTotalList[0]);
            Assert.AreEqual(36, result.RunningTotalList[1]);
            Assert.AreEqual(42, result.RunningTotalList[2]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(numPins, result.ResultList[2].ScoreCells[0]);
            
            
        }

        [TestMethod]
        public async Task UpdateScoreStrikeMissRegularShotTest()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10, 10 };
            
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(16 + 0 + 6, result.TotalScore);
            Assert.AreEqual(16, result.RunningTotalList[0]);
            Assert.AreEqual(22, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(numPins, result.ResultList[1].ScoreCells[1]);
            
        }
        
        [TestMethod]
        public async Task UpdateScoreStrikeRegularShotMiss()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 14;
            var oldRunningTotalList = new List<int> { 10, 14 };
           
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 }  },
                new Frame {ScoreCells = new List<int>() { 4 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(18, result.TotalScore);
            Assert.AreEqual(14, result.RunningTotalList[0]);
            Assert.AreEqual(18, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(0, result.ResultList[1].ScoreCells[1]);
            
        }

        [TestMethod]
        public async Task UpdateScoreStrikeMissMiss()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10, 10 };            
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(10, result.TotalScore);
            Assert.AreEqual(10, result.RunningTotalList[0]);
            Assert.AreEqual(10, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(0, result.ResultList[1].ScoreCells[1]);
            
        }

        [TestMethod]
        public async Task UpdateScoreStrikeMissSpare()
        {
            var playerScore = new PlayerGameData();
            var oldTotalScore = 10;
            var oldRunningTotalList = new List<int> { 10, 10 };            
            var oldFrames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0 } }
            };

            var oldPlayerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);
            playerScore = CreatePlayerData(oldTotalScore, oldRunningTotalList, oldFrames);

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(20 + 10, result.TotalScore);
            Assert.AreEqual(20, result.RunningTotalList[0]);
            Assert.AreEqual(30, result.RunningTotalList[1]);
            Assert.AreEqual(oldPlayerScore.ResultList[0].ScoreCells[0], result.ResultList[0].ScoreCells[0]);
            Assert.AreEqual(oldPlayerScore.ResultList[1].ScoreCells[0], result.ResultList[1].ScoreCells[0]);
            Assert.AreEqual(10, result.ResultList[1].ScoreCells[1]);
            
        }

        [TestMethod]
        public async Task UpdateScoreBonusFrameOneShotTest()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 5;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(142, result.TotalScore);
            Assert.AreEqual(142, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreSecondShotInBonusFrameTest()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 5 } });
            playerScore.TotalScore += 5;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 5);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 3;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(145, result.TotalScore);
            Assert.AreEqual(145, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(5, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreThirdShotBonusFrameTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreTripleStrikeBonusFrameTest()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 10, 10 } });
            playerScore.TotalScore += 20;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 10 + 10);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(167, result.TotalScore);
            Assert.AreEqual(167, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreDoubleStrikeBonusFrameStrikeFrameBeforeTest()
        {
        }

        [TestMethod]
        public async Task UpdateScoreDoubleMissBonusFrameTest()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 0 } });
            playerScore.TotalScore += 0;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8]);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(137, result.TotalScore);
            Assert.AreEqual(137, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(0, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeSpareBonusFrame()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 10, 4 } });
            playerScore.TotalScore += 14;
            playerScore.RunningTotalList.Add(playerScore.TotalScore);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(157, result.TotalScore);
            Assert.AreEqual(157, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[0]);
            Assert.AreEqual(4, result.ResultList[9].ScoreCells[1]);
        }

        [TestMethod]
        public async Task UpdateScoreSpareStrikeBonusFrame()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 4, 6 } });
            playerScore.TotalScore += 10;
            playerScore.RunningTotalList.Add(playerScore.TotalScore);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(157, result.TotalScore);
            Assert.AreEqual(157, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
            Assert.AreEqual(4, result.ResultList[9].ScoreCells[0]);
            Assert.AreEqual(6, result.ResultList[9].ScoreCells[1]);
        }

        [TestMethod]
        public async Task UpdateScoreSpareRegularShotBonus()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 4, 6 } });
            playerScore.TotalScore += 10;
            playerScore.RunningTotalList.Add(playerScore.TotalScore);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 5;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(152, result.TotalScore);
            Assert.AreEqual(152, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
            Assert.AreEqual(4, result.ResultList[9].ScoreCells[0]);
            Assert.AreEqual(6, result.ResultList[9].ScoreCells[1]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeStrikeRegularBonusFrame()
        {
            var playerScore = AllNonBonusFramesFilled();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 10, 10 } });
            playerScore.TotalScore += 20;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 10 + 10);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 5;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(162, result.TotalScore);
            Assert.AreEqual(162, result.RunningTotalList[9]);
            Assert.AreEqual(137, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeBeforeTwoStrikesInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledStrikeAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 10 } });
            playerScore.TotalScore += 10;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 10);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(180, result.TotalScore);
            Assert.AreEqual(180, result.RunningTotalList[9]);
            Assert.AreEqual(160, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeBeforeThreeStrikesInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledStrikeAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 10, 10 } });
            playerScore.TotalScore = 180; //20 for strike in frame before bonus and another for 2 20s
            playerScore.RunningTotalList[8] = 160;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 10);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(190, result.TotalScore);
            Assert.AreEqual(190, result.RunningTotalList[9]);
            Assert.AreEqual(160, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(10, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeBeforeSpareInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledStrikeAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 4 } });
            playerScore.TotalScore += 4;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 4);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 6;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(160, result.TotalScore);
            Assert.AreEqual(160, result.RunningTotalList[9]);
            Assert.AreEqual(150, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(4, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreStrikeBeforeTwoRegularShotsInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledStrikeAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            playerScore.ResultList.Add(new Frame { ScoreCells = new List<int> { 4 } });
            playerScore.TotalScore += 4;
            playerScore.RunningTotalList.Add(playerScore.RunningTotalList[8] + 4);

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 4;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(156, result.TotalScore);
            Assert.AreEqual(156, result.RunningTotalList[9]);
            Assert.AreEqual(148, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[1]);
            Assert.AreEqual(4, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreTwoStrikesBeforeOneStrikeInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledTwoStrikesAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(184, result.TotalScore);
            Assert.AreEqual(184, result.RunningTotalList[9]);
            Assert.AreEqual(174, result.RunningTotalList[8]);
            Assert.AreEqual(164, result.RunningTotalList[7]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreSpareBeforeStrikeInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledSpareAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(160, result.TotalScore);
            Assert.AreEqual(160, result.RunningTotalList[9]);
            Assert.AreEqual(150, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreSpareBeforeRegularShotInBonusFrame()
        {

            var playerScore = AllNonBonusFramesFilledSpareAtEnd();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 4;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(148, result.TotalScore);
            Assert.AreEqual(148, result.RunningTotalList[9]);
            Assert.AreEqual(144, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[0]);
        }

        [TestMethod]
        public async Task UpdateScoreAllMissTest()
        {
            var playerScore = GetFramesFilledExceptOneAllZeroes();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 0;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(0, result.TotalScore);
            Assert.AreEqual(0, result.RunningTotalList[9]);
            Assert.AreEqual(0, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
        }

        [TestMethod]
        public async Task UpdateScoreAllStrikesTest()
        {
            var playerScore = GetFramesFilledExceptOneAllStrikes();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 10;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(300, result.TotalScore);
            Assert.AreEqual(300, result.RunningTotalList[9]);
            Assert.AreEqual(270, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
        }

        [TestMethod]
        public async Task UpdateScoreAllSparesTest()
        {
            var playerScore = GetFramesFilledExceptOneAllSpares();

            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(playerScore));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 4;
            var result = await helper.UpdateScore(PLAYER_ID, numPins);

            Assert.AreEqual(116, result.TotalScore);
            Assert.AreEqual(116, result.RunningTotalList[9]);
            Assert.AreEqual(102, result.RunningTotalList[8]);
            Assert.AreEqual(numPins, result.ResultList[9].ScoreCells[2]);
        }
        

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumOneScoreTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 4 } };
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 4 }
                }               
            };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual(result.Item2, -1);
            Assert.AreEqual(result.Item1, 0);
        }      

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumTwoScoreTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 2, 4 } };
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                }               
            };

            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 2);

            Assert.AreEqual(2, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumThreeScoreTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 2, 4 }, new List<int>() { 5 } };
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(4, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumSpareFristFrameTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 5, 10 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 5, 5 }
                }
            };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual(0, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumSpareSecondFrameTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 2, 4 }, new List<int>() { 5, 10 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5, 5 }
                }
            };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 2);

            Assert.AreEqual(5, result.Item1);
            Assert.AreEqual(2, result.Item2);
        }


        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumStrikeFirstFrame()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 10, -1 } };
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                }               
            };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual(0, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task GetScorOneShotsBackAndFrameNumStrikeSecondFrameTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 2, 4 }, new List<int>() { 10, -1 } };
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                }
            };
            var result = playersHelper.GetScoreOneShotBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(4, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }
        //todo! go back and change it        
        //GetScoreTwoShotsBackTestsdone in Context of finding Strike
        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumFirstShotTest()
        {
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 1, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumSecondShotTest()
        {
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 1, 2);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumThirdShotTest()
        {
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumFourthShotTest()
        {
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 2, 4 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5, 3 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 2, 2);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumSpareShotBeforeTest()
        {
            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 4, 6 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumStrikeShotBeforeTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 10, -1 }, new List<int>() { 5 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }       

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumForthShotStrikeFirstFrameTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 10, -1 }, new List<int>() { 5 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 2, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod]
        public async Task FoundStrikeTwoShotsBackAndFrameNumStrikeStrikeShotTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { 10, -1 }, new List<int>() { 10, -1 }, new List<int>() { 5 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 3, 1);

            Assert.AreEqual(true, result.Item1);
            Assert.AreEqual(1, result.Item2);
        }

        [TestMethod] ///CONTRADICTION!!!
        public async Task FoundStrikeTwoShotsBackAndFrameNumSpareStrikeShotTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { -1, 10 }, new List<int>() { 10, -1 }, new List<int>() { 5 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 4, 6 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 5 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 3, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }

        [TestMethod] ///CONTRADICTION!!!
        public async Task FoundStrikeTwoShotsBackAndFrameNumThirdShotAfterStrikeTest()
        {
            //var resultList = new List<List<int>>() { new List<int>() { -1, 10 }, new List<int>() { 10, -1 }, new List<int>() { 5 } };

            var resultList = new List<Frame>() {
                new Frame {
                    ScoreCells = new List<int>() { 10 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 4, 5 }
                },
                new Frame {
                    ScoreCells = new List<int>() { 3 }
                }
            };

            var result = playersHelper.FoundStrikeTwoShotsBackAndFrameNum(resultList, 3, 1);

            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual(-1, result.Item2);
        }
        //bonus frame tests
        private static PlayerGameData CreatePlayerData(int totalScore, List<int> runningTotal, List<int> frame)
        {
            return new PlayerGameData
            {
                TotalScore = totalScore,
                RunningTotalList = runningTotal,
                ResultList = new List<Frame>() { new Frame { ScoreCells = frame } }
            };
        }

        private static PlayerGameData CreatePlayerData(int totalScore, List<int> runningTotal, List<Frame> frames)
        {
            return new PlayerGameData
            {
                TotalScore = totalScore,
                RunningTotalList = runningTotal,
                ResultList = frames
            };
        }

        private static PlayerGameData AllNonBonusFramesFilled()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0, 10 } },
                new Frame {ScoreCells = new List<int>() { 5, 4 } },
                new Frame {ScoreCells = new List<int>() { 3, 7 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 1, 9 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4, 4 } },
                new Frame {ScoreCells = new List<int>() { 2, 5 } }
            };

            return new PlayerGameData
            {
                TotalScore = 137,
                RunningTotalList = new List<int> { 20, 35, 44, 64, 84, 104, 122, 130, 137 },
                ResultList = frames
            };
        }

        private static PlayerGameData AllNonBonusFramesFilledStrikeAtEnd()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0, 10 } },
                new Frame {ScoreCells = new List<int>() { 5, 4 } },
                new Frame {ScoreCells = new List<int>() { 3, 7 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 1, 9 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4, 4 } },
                new Frame {ScoreCells = new List<int>() { 10 } }
            };

            return new PlayerGameData
            {
                TotalScore = 140,
                RunningTotalList = new List<int> { 20, 35, 44, 64, 84, 104, 122, 130, 140 },
                ResultList = frames
            };
        }

        private static PlayerGameData AllNonBonusFramesFilledTwoStrikesAtEnd()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0, 10 } },
                new Frame {ScoreCells = new List<int>() { 5, 4 } },
                new Frame {ScoreCells = new List<int>() { 3, 7 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 1, 9 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10 } }
            };

            return new PlayerGameData
            {
                TotalScore = 154,
                RunningTotalList = new List<int> { 20, 35, 44, 64, 84, 104, 134, 144, 154 },
                ResultList = frames
            };
        }

        private static PlayerGameData AllNonBonusFramesFilledSpareAtEnd()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 0, 10 } },
                new Frame {ScoreCells = new List<int>() { 5, 4 } },
                new Frame {ScoreCells = new List<int>() { 3, 7 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 1, 9 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 4, 4 } },
                new Frame {ScoreCells = new List<int>() { 4, 6 } }
            };

            return new PlayerGameData
            {
                TotalScore = 140,
                RunningTotalList = new List<int> { 20, 35, 44, 64, 84, 104, 122, 130, 140 },
                ResultList = frames
            };
        }

        private static PlayerGameData GetFramesFilledExceptOneAllZeroes()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 0, 0 } },
                new Frame {ScoreCells = new List<int>() { 0, 0 } },
                new Frame {ScoreCells = new List<int>() { 0, 0  } },
                new Frame {ScoreCells = new List<int>() { 0, 0  } },
                new Frame {ScoreCells = new List<int>() { 0, 0 } },
                new Frame {ScoreCells = new List<int>() { 0, 0  } },
                new Frame {ScoreCells = new List<int>() { 0, 0 } },
                new Frame {ScoreCells = new List<int>() { 0, 0  } },
                new Frame {ScoreCells = new List<int>() { 0, 0 } },
                new Frame {ScoreCells = new List<int>() { 0, 0 } }
            };

            return new PlayerGameData
            {
                TotalScore = 0,
                RunningTotalList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                ResultList = frames
            };
        }

        private static PlayerGameData GetFramesFilledExceptOneAllSpares()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 4, 6 } },
                new Frame {ScoreCells = new List<int>() { 3, 7 } },
                new Frame {ScoreCells = new List<int>() { 1, 9  } },
                new Frame {ScoreCells = new List<int>() { 0, 10  } },
                new Frame {ScoreCells = new List<int>() { 3, 7  } },
                new Frame {ScoreCells = new List<int>() { 1, 9   } },
                new Frame {ScoreCells = new List<int>() { 0, 10 } },
                new Frame {ScoreCells = new List<int>() { 3, 7   } },
                new Frame {ScoreCells = new List<int>() { 1, 9  } },
                new Frame {ScoreCells = new List<int>() { 0, 10 } }
            };

            return new PlayerGameData
            {
                TotalScore = 112,
                RunningTotalList = new List<int> { 13, 24, 34, 47, 58, 68, 81, 92, 102, 112 },
                ResultList = frames
            };
        }

        private static PlayerGameData GetFramesFilledExceptOneAllStrikes()
        {
            var frames = new List<Frame>() {
                new Frame { ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10  } },
                new Frame {ScoreCells = new List<int>() { 10  } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10  } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10  } },
                new Frame {ScoreCells = new List<int>() { 10 } },
                new Frame {ScoreCells = new List<int>() { 10, 10 } }
            };

            return new PlayerGameData
            {
                TotalScore = 290,
                RunningTotalList = new List<int> { 30, 60, 90, 120, 150, 180, 210, 240, 270, 290 },
                ResultList = frames
            };
        }
    }
}
