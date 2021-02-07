using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("game")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;

        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        [HttpGet("newopponent")]
        public void NewOponentGame()
        {
            //SkapaSocket
            //New game
            //FirstThrow
        }

        [HttpGet("newaigame")]
        public void NewAiGame()
        {
            
        }
    }
}
