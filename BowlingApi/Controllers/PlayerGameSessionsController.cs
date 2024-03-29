﻿using System;
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
    public class PlayerGameSessionsController : ControllerBase
    {
        private readonly IPlayerGameSessionsHelper _playersHelper;

        public PlayerGameSessionsController(IPlayerGameSessionsHelper playersHelper)
        {
            _playersHelper = playersHelper;
        }                

        [HttpPost("")]
        public async Task<ActionResult<PlayerGameSessionOut>> Post([FromBody] PlayerGameSessionIn playerGameSession)
        {           
            try
            {
                var result = await _playersHelper.InsertPlayerGameSession(playerGameSession);
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
        public async Task<ActionResult> Put(string Id, [FromBody]PlayerGameSessionIn playerGameData)
        {
            if (!Guid.TryParse(Id, out var gameSessionIdGuid))
            {
                return StatusCode(400, "player game session: " + Id + " is not a Guid");
            }

            try
            {
                var result = await _playersHelper.ReplacePlayerGameSession(playerGameData, gameSessionIdGuid);
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
            if(!Guid.TryParse(Id, out var gameSessionIdGuid))
            {
                return StatusCode(400, "player game session Id: " + Id + " is not a Guid");
            }

            try
            {
                var result = await _playersHelper.UpdateScore(gameSessionIdGuid, numPins);
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
                Console.WriteLine("Error in {Id}/calculateNewScore action ", e);
                return StatusCode(500);
            }
        }
        
        [HttpGet("{Id}")]
        public async Task<ActionResult<PlayerGameSessionOut>> Get(string Id) //return new total
        {
            if (!Guid.TryParse(Id, out var gameSessionIdGuid))
            {
                return StatusCode(400, "player game session: " + Id + " is not a Guid");
            }

            try
            {
                var result = await _playersHelper.GetPlayerGameSession(gameSessionIdGuid);
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

        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(string playerGameSessionId)
        {
            if (!Guid.TryParse(playerGameSessionId, out var playerGameSessionIdGuid))
            {
                return StatusCode(400, "player game session: " + playerGameSessionId + " is not a Guid");
            }
            try
            {
                var deleted = await _playersHelper.DeletePlayerGameSession(playerGameSessionIdGuid);
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