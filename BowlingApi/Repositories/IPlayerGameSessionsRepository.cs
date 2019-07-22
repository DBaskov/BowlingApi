using BowlingApi.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repository
{
    public interface IPlayerGameSessionsRepository
    {
        Task<bool> AddPlayers(List<PlayerGameSession> players);

        Task<bool> Add(PlayerGameSession player);

        Task<bool> DeletePlayers(List<string> playerIds);

        Task<bool> Delete(string playerId);

        Task<bool> Update(PlayerGameSession updatedGameData);

        Task<bool> Replace(PlayerGameSession playerGameDataToReplace);

        Task<PlayerGameSession> Get(string playerId);
    }
}
