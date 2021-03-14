using Backend.Dto.rest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    public class InviteController : AuthorizedController
    {
        private readonly ILogger<GameManager> logger;
        public InviteController(ILogger<GameManager> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("/api/invite/create")]
        public InviteResponseDto CreateInvite()
        {
            var id = GamesService.CreateInvite(logger, GetUserId());

            logger.LogInformation("Creating game invite");
            return new InviteResponseDto { gameId = id.ToString() };
        }
    }
}
