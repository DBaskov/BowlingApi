using BowlingApi.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repository
{
    public interface IPlayersDataRepository
    {
        Task<bool> AddPlayers(List<PlayerGameSession> players);

        Task<bool> AddPlayer(PlayerGameSession player);

        Task<bool> DeletePlayers(List<string> playerIds);

        Task<bool> DeletePlayer(string playerId);

        Task<bool> UpdatePlayerData(PlayerGameSession updatedGameData);

        Task<bool> ReplacePlayerData(PlayerGameSession playerGameDataToReplace);

        Task<PlayerGameSession> GetPlayerData(string playerId);
    }
}
