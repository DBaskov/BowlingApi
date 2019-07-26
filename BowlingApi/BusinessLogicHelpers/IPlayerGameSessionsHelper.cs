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
        Task<PlayerGameSession> GetPlayerGameSession(Guid gameSessionId);

        Task<PlayerGameSession> InsertPlayerGameSession(PlayerGameSessionIn playerName);

        Task<bool> ReplacePlayerGameSession(PlayerGameSessionIn playerGameData, Guid gameSessionId);

        Task<PlayerGameSession> UpdateScore(Guid gameSessionId, int numPins);

        Task<List<PlayerGameSession>> ChangeFrameScore(Guid gameSessionId, int frameNumber, int newScore);        

        Task<bool> DeletePlayerGameSession(Guid gameSessionId);
    }
}
