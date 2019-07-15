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
    [Route("api/bowling/v1/players")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
        private readonly IPlayersHelper _playersHelper;

        public BowlingController(IPlayersHelper playersHelper)
        {
            _playersHelper = playersHelper;
        }                

        [HttpPost("")] //check model state
        public async Task<ActionResult<PlayerGameDataOut>> CreatePlayer([FromBody] string playerName)
        {           
            try
            {
                var result = await _playersHelper.InstiateAndInsertPlayerGameData(playerName);
                return StatusCode(201, new PlayerGameDataOut
                {
                    PlayerId = result.PlayerId,
                    PlayerName = result.PlayerName,
                    TotalScore = result.TotalScore,
                    ResultList = result.ResultList,
                    RunningTotalList = result.RunningTotalList
                });
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("{playerId}")] //for editing score
        public async Task<ActionResult> PutPlayerGameData(string playerId, [FromBody]PlayerGameDataIn playerGameData) //return new total
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
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("{playerId}/calculateNewScore")] //every time bowling pins are knocked down
        public async Task<ActionResult<PlayerGameDataOut>> CalculateNewScore(string playerId, [FromBody]int numPins) //return new total
        {           
            if(!Guid.TryParse(playerId, out var playerIdGuid))
            {
                return StatusCode(400, "playerId: " + playerId + " is not in proper format");
            }

            try
            {
                var result = await _playersHelper.UpdateScore(playerIdGuid, numPins);
                return Ok(new PlayerGameDataOut
                {
                    PlayerId = result.PlayerId,
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
                return StatusCode(500);
            }
        }
        
        [HttpGet("{playerId}")] //after score been edited, we would need to fetch again
        public async Task<ActionResult<PlayerGameDataOut>> PlayerDataGet(string playerId) //return new total
        {
            if (!Guid.TryParse(playerId, out var playerIdGuid))
            {
                return StatusCode(400, "playerId: " + playerId + " is not in proper format");
            }

            try
            {
                var result = await _playersHelper.GetPlayerGameData(playerIdGuid);
                return Ok(new PlayerGameDataOut
                {
                    PlayerId = result.PlayerId,
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
                return StatusCode(500);
            }
        }               

        [HttpDelete("{playerId}")]
        public async Task<ActionResult> DeletePlayer(string playerId)
        {
            if (!Guid.TryParse(playerId, out var playerIdGuid))
            {
                return StatusCode(400, "playerId: " + playerId + " is not in proper format");
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
            catch(Exception)
            {
                return StatusCode(500);
            }
        }
               
    }
}