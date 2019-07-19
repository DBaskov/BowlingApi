using System;
using System.Reflection;
using System.Threading.Tasks;
using Bowling.Api.DTOs;
using BowlingApi.DBContexts;
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

        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _mongoDbContext.ConnectionOk();
                return Ok(new StatusCheck
                {
                    AppName = "Bowling.Api",
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    MongoDbWorking = result
                });
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception happened when checking connection status of mongo: ", e);

                return StatusCode(500);
            }
        } 
    }
}