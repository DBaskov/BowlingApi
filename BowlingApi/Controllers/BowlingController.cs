using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bowling.Api.DTOs;
using BowlingApi.BusinessLogicHelpers;
using BowlingApi.Common.CustomExceptions;
using BowlingApi.DTOs;
using BowlingApi.Repositories.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BowlingApi.Controllers
{
    [Route("api/bowling/v1/player-game-sessions")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
        private readonly IPlayersHelper _playersHelper;

        public BowlingController(IPlayersHelper playersHelper)
        {
            _playersHelper = playersHelper;
        }                

        [HttpPost("")] //check model state
        public async Task<ActionResult<PlayerGameSessionOut>> CreatePlayer([FromBody] string playerName)
        {           
            try
            {
                var result = await _playersHelper.InstiateAndInsertPlayerGameData(playerName);
                return StatusCode(201, new PlayerGameSessionOut
                {
                    PlayerGameSessionId = result.PlayerGameSessionId,
                    PlayerName = result.PlayerName,
                    TotalScore = result.TotalScore,
                    ResultList = result.ResultList,
                    RunningTotalList = result.RunningTotalList
                });
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in Post action", e);
                return StatusCode(500);
            }
        }

        [HttpPut("{Id}")] //for editing score
        public async Task<ActionResult> PutPlayerGameData(string Id, [FromBody]PlayerGameDataIn playerGameData) //return new total
        {
            try
            {
                var result = await _playersHelper.ReplacePlayerGameData(playerGameData);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Put action", e);
                return StatusCode(500);
            }
        }

        [HttpPost("{Id}/calculate-new-score")] //every time bowling pins are knocked down
        public async Task<ActionResult<PlayerGameSessionOut>> CalculateNewScore(string Id, [FromBody]int numPins) //return new total
        {           
            if(!Guid.TryParse(Id, out var playerIdGuid))
            {
                return StatusCode(400, "player game session Id: " + Id + " is not in proper format");
            }

            try
            {
                var result = await _playersHelper.UpdateScore(playerIdGuid, numPins);
                return Ok(new PlayerGameSessionOut
                {
                    PlayerGameSessionId = result.PlayerGameSessionId,
                    PlayerName = result.PlayerName,
                    TotalScore = result.TotalScore,
                    ResultList = result.ResultList,
                    RunningTotalList = result.RunningTotalList
                });
            }
            catch(ItemNotFoundInMongoException)
            {
                return NotFound();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in {playerId}/calculateNewScore action ", e);
                return StatusCode(500);
            }
        }
        
        [HttpGet("{Id}")] //after score been edited, we would need to fetch again
        public async Task<ActionResult<PlayerGameSessionOut>> PlayerDataGet(string Id) //return new total
        {
            if (!Guid.TryParse(Id, out var playerIdGuid))
            {
                return StatusCode(400, "player game session: " + Id + " is not in proper format");
            }

            try
            {
                var result = await _playersHelper.GetPlayerGameData(playerIdGuid);
                return Ok(new PlayerGameSessionOut
                {
                    PlayerGameSessionId = result.PlayerGameSessionId,
                    PlayerName = result.PlayerName,
                    TotalScore = result.TotalScore,
                    ResultList = result.ResultList,
                    RunningTotalList = result.RunningTotalList
                }); 
            }
            catch (ItemNotFoundInMongoException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Get action", e);
                return StatusCode(500);
            }
        }               

        [HttpDelete("{playerId}")]
        public async Task<ActionResult> DeletePlayer(string playerId)
        {
            if (!Guid.TryParse(playerId, out var playerIdGuid))
            {
                return StatusCode(400, "player game session: " + playerId + " is not in proper format");
            }
            try
            {
                var deleted = await _playersHelper.DeletePlayerGameData(playerId);
                if (deleted)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in Delete action", e);
                return StatusCode(500);
            }
        }
               
    }
}