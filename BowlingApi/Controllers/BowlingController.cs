using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BowlingApi.DTOs;
using BowlingApi.PlayersHelper;
using BowlingApi.Services;
using BowlingApi.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BowlingApi.Controllers
{
    [Route("api/bowling/v1")]
    [ApiController]
    public class BowlingController : ControllerBase
    {
        private readonly IPlayersHelper _playersHelper;

        public BowlingController(IPlayersHelper playersHelper)
        {
            _playersHelper = playersHelper;
        }
        /*
        [HttpPost]
        public async Task<ActionResult<MatchStartOut>> MatchStartPost([FromBody] List<string> playerNames)
        {
            throw new NotImplementedException();
        } */

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
        }

        /*
        [HttpGet("{id}/scores")]
        public async Task<ActionResult<List<PlayerScoreInfo>>> PlayersScoreInfoGet(string id)
        {
            throw new NotImplementedException();
        } */

        [HttpPut("players/{playerId}/scores/{cellId}")] //for editing score

        //[HttpGet("{playerId}/scores/{cellId}")] //after score been edited

        [HttpPatch("players/{playerId}/scores")] //every time bowling pins are knocked down
        public async Task<ActionResult<ScoresOnCurrentFrameOut>> NewScorePut(string playerId, [FromBody]int numPins) //return new total
        {
            throw new NotImplementedException();
        }

        [HttpGet("players/{playerId}/scores")] //after score been edited, we would need to fetch again
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
        
        [HttpDelete("bulkPlayers")]
        public async Task<ActionResult> DeleteList([FromBody]List<string> playerIds)
        {
            throw new NotImplementedException();
        }
        
    }
}