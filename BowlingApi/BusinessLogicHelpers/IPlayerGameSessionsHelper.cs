using Bowling.Api.DTOs;
using BowlingApi.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.BusinessLogicHelpers
{
    public interface IPlayerGameSessionsHelper
    {
        Task<List<PlayerGameSession>> InstatiateBulkPlayerGameData(List<string> playerNames);

        Task<PlayerGameSession> GetPlayerGameData(Guid playerId);

        Task<PlayerGameSession> InstiateAndInsertPlayerGameData(string playerName);

        Task<bool> ReplacePlayerGameData(PlayerGameDataIn playerGameData);

        Task<PlayerGameSession> UpdateScore(Guid playerId, int numPins);

        Task<List<PlayerGameSession>> ChangeFrameScore(string playerId, int frameNumber, int newScore);
        
        Task<bool> DeleteBulkPlayerGameData(List<Guid> playerIds);

        Task<bool> DeletePlayerGameData(Guid playerIds);
    }
}
