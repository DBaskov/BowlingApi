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
        Task<List<PlayerGameSession>> InstatiateBulkPlayerGameSession(List<string> playerNames);

        Task<PlayerGameSession> GetPlayerGameSession(Guid playerId);

        Task<PlayerGameSession> InstiateAndInsertPlayerGameSession(string playerName);

        Task<bool> ReplacePlayerGameSession(PlayerGameDataIn playerGameData);

        Task<PlayerGameSession> UpdateScore(Guid playerId, int numPins);

        Task<List<PlayerGameSession>> ChangeFrameScore(string playerId, int frameNumber, int newScore);
        
        Task<bool> DeleteBulkPlayerGameSession(List<Guid> playerIds);

        Task<bool> DeletePlayerGameSession(Guid playerIds);
    }
}
