using BowlingApi.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BowlingApi.Services
{
    public interface IPlayersDataService
    {
        Task<bool> AddPlayers(List<PlayerGameData> players);

        Task<bool> DeletePlayers(List<string> playerIds);

        Task<bool> DeletePlayer(string playerId);

        Task<PlayerGameData> UpdatePlayerData(PlayerGameData updatedGameData);

        Task<PlayerGameData> GetPlayerData(string playerId);
    }
}
