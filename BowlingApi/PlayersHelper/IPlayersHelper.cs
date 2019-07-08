using BowlingApi.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.PlayersHelper
{
    public interface IPlayersHelper
    {
        Task<List<PlayerGameData>> InstatiateBulkPlayerGameData(List<string> playerNames);

        Task<List<PlayerGameData>> UpdateScore(int numPins);

        Task<List<PlayerGameData>> ChangeFrameScore(string playerId, int frameNumber, int newScore);
        
        Task<bool> DeleteBulkPlayerGameData(List<string> playerIds);
    }
}
