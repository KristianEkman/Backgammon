using Backend.Dto.toplist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    public class ToplistController : AuthorizedController
    {
        [Route("api/toplist")]
        [HttpGet]
        public Toplist GetToplist()
        {
            var userId = GetUserId();

            // todo: separate entry for this user.
            // top 10.
            using (var db = new Db.BgDbContext())
            {
                var list = db.Users
                    .OrderByDescending(u => u.Elo)
                    .Where(u => u.Id != Guid.Empty) // filter quest
                    .Take(10)
                    .Select(user => new ToplistResult { 
                        name = user.Name, 
                        games = user.Players.Count(), 
                        elo = user.Elo })
                    .ToArray();
                for (int i = 0; i < list.Length; i++)
                    list[i].place = i + 1;
                var topList = new Toplist { 
                Results = list};
                return topList;
            }
        }
    }
}
