using BowlingApi.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Repository
{
    public interface IPlayersDataRepository
    {
        Task<bool> AddPlayers(List<PlayerGameData> players);

        Task<bool> AddPlayer(PlayerGameData player);

        Task<bool> DeletePlayers(List<string> playerIds);

        Task<bool> DeletePlayer(string playerId);

        Task<bool> UpdatePlayerData(PlayerGameData updatedGameData);

        Task<bool> ReplacePlayerData(PlayerGameData playerGameDataToReplace);

        Task<PlayerGameData> GetPlayerData(string playerId);
    }
}
