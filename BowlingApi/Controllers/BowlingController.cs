using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingApi.BusinessLogicHelpers;
using BowlingApi.DTOs;
using BowlingApi.Services;
using BowlingApi.Services.Models;
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
        /*
        [HttpPost("bulkPlayers")]
        public async Task<ActionResult<List<PlayerStartInfoOut>>> CreatePlayersPost([FromBody]List<string> playerNames)
        {           
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _playersHelper.InstatiateBulkPlayerGameData(playerNames);

                return Ok(result);

            } catch(Exception)
            {
                return StatusCode(500);
            }
        } */

        [HttpPost("")] //check model state
        public async Task<ActionResult<PlayerGameData>> CreatePlayer([FromBody] string playerName)
        {           
            try
            {
                var result = await _playersHelper.InstiateAndInsertPlayerGameData(playerName);
                return StatusCode(201, result);
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }
    
        /*
        [HttpGet("{id}/scores")]
        public async Task<ActionResult<List<PlayerScoreInfo>>> PlayersScoreInfoGet(string id)
        {
            throw new NotImplementedException();
        } */

        [HttpPut("players/{playerId}/scores/{cellId}")] //for editing score

        //[HttpGet("{playerId}/scores/{cellId}")] //after score been edited

        [HttpPost("{playerId}/calculateNewScore")] //every time bowling pins are knocked down
        public async Task<ActionResult<PlayerGameData>> CalculateNewScore(string playerId, [FromBody]int numPins) //return new total
        {           
            if(Guid.TryParse(playerId, out var playerIdGuid))
            {
                return StatusCode(400, "playerId: " + playerId + " is not in proper format");
            }

            try
            {
                var result = await _playersHelper.UpdateScore(playerIdGuid, numPins);
                return Ok(result);
            }
            catch(ItemNotFoundInMongoException)
            {
                return NotFound();
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{playerId}/scores")] //after score been edited, we would need to fetch again
        public async Task<ActionResult<ScoresOnCurrentFrameOut>> NewScoreGet(string playerId) //return new total
        {
            throw new NotImplementedException();
        }

        [HttpPut("bulkPlayers")]
        public async Task<ActionResult> NewGamePut([FromBody]List<string> playerIds)
        {
            throw new NotImplementedException();
        }

        /*
        [HttpGet("{id}/players/{id}/scores/current")]
        public async Task<ActionResult<ScoresOnCurrentFrameOut>> ScoreCurrentGet()
        {
            throw new NotImplementedException();
        } */

        [HttpDelete("players/{playerId}")]
        public async Task<ActionResult> DeletePlayer(string playerId)
        {
            throw new NotImplementedException();
        }
        
        
    }
}