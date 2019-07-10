using BowlingApi.BusinessLogicHelpers;
using BowlingApi.Services;
using BowlingApi.Services.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Bowling.Api.Tests
{
    [TestClass]
    public class PlayersHelperTests
    {
        private Mock<IPlayersDataService> mockPlayersDataService;
        private PlayersHelper playersHelper;

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

        [TestMethod]
        public async Task UpdateScoreSingleValueTest()
        {
            mockPlayersDataService.Setup(x => x.GetPlayerData(It.IsAny<string>())).Returns(Task.FromResult(new PlayerGameData()));
            mockPlayersDataService.Setup(x => x.UpdatePlayerData(It.IsAny<PlayerGameData>())).Returns(Task.FromResult(true));

            var helper = new PlayersHelper(mockPlayersDataService.Object);
            var numPins = 4;
            var result = await helper.UpdateScore(4);

            Assert.AreEqual(result.TotalScore, numPins);
            Assert.AreEqual(result.FrameScores.Frames[0].Scores.Item1, numPins);
        }

        [TestMethod]
        public async Task UpdateScoreSecondValueTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreThirdValueTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreMissOneShotTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreMissBothShotsTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSpareTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreShotAfterSpareTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreStrikeAfterSpareTest()
        {

        }

        [TestMethod]
        public async Task UpdateScoreSpareAfterMissTest()
        {

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
    }
}
