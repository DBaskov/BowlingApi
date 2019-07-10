using BowlingApi.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.BusinessLogicHelpers
{
    public interface IPlayersHelper
    {
        Task<List<PlayerGameData>> InstatiateBulkPlayerGameData(List<string> playerNames);

        Task<PlayerGameData> InstiateAndInsertPlayerGameData(string playerName);

        Task<List<PlayerGameData>> UpdateScore(int numPins);

        Task<List<PlayerGameData>> ChangeFrameScore(string playerId, int frameNumber, int newScore);
        
        Task<bool> DeleteBulkPlayerGameData(List<string> playerIds);

        Task<bool> DeletePlayerGameData(string playerIds);
    }
}
