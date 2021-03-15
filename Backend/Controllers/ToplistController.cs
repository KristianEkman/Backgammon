using Backend.Db;
using Backend.Dto.toplist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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

            using (var db = new Db.BgDbContext())
            {
                var top10 = db.Users
                    .OrderByDescending(u => u.Elo)
                    .Where(u => u.Id != Guid.Empty && u.Name != "deleted") // filter guest and deleted
                    .Take(10)
                    .Select(user => new
                    {
                        id = user.Id,
                        name = user.Name,
                        elo = user.Elo,
                    })
                    .ToArray();

                var results = new ToplistResult[top10.Length];

                for (int i = 0; i < top10.Length; i++)
                {
                    results[i] = new ToplistResult
                    {
                        place = i + 1,
                        name = top10[i].name,
                        elo = top10[i].elo,
                        you = top10[i].id.ToString() == userId
                    };
                }
                var meEnt = db.Users.Single(u => u.Id.ToString() == userId);

                // This has to be the last call, because the connection then will be closed.
                var myRank = db.ExcecuteScalar("GetRank", ("Id", userId));  // to prevent reading entire table into this app a stored procedure is called on the server.
                var me = new ToplistResult()
                {
                    elo = meEnt.Elo,
                    name = meEnt.Name,
                    place = myRank
                };
                var topList = new Toplist { results = results };

                return topList;
            }
        }
    }
}
