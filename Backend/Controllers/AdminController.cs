using Backend.Dto;
using Backend.Dto.admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    public class AdminController : AuthorizedController
    {
        [Route("api/admin")]
        [HttpGet]
        public PlayedGameListDto AllGames(string afterDate)
        {
            AssertAdmin();
            DateTime date;
            if (!DateTime.TryParse(afterDate, out date))
                date = DateTime.Parse("1900-01-01");

            // a trick to prevent loading same game twice.
            date = date.AddMilliseconds(1);

            using (var db = new Db.BgDbContext())
            {
                var playedGames = db.Games.Select(g => new
                {
                    started = g.Started,
                    black = g.Players.Where(p => p.Color == Db.Color.Black),
                    white = g.Players.Where(p => p.Color == Db.Color.White),
                    winner = g.Winner
                }).Select(pl => new PlayedGameDto {
                    started = pl.started,
                    black = pl.black.First().User.Name,
                    white = pl.white.First().User.Name,
                    winner = (PlayerColor)pl.winner
                })
                .Where(x => x.started > date)
                .OrderBy(x => x.started)
                .Take(30);
               
                
                var games = playedGames.ToArray();

                return new PlayedGameListDto
                {
                    games = games
                };
            }
        }        
    }
}
