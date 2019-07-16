using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bowling.Api.DTOs;
using BowlingApi.DBContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bowling.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        readonly IMongoDBContext _mongoDbContext;

        public HealthCheckController(IMongoDBContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        /*
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _mongoDbContext.ConnectionOk;
            }
            catch()
            {

            }

            var statusResponse = new StatusCheck
            {
                AppName = "Bowling.Api",
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
            }
        } */
    }
}