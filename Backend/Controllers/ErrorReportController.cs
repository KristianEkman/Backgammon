using Backend.Db;
using Backend.Dto;
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
    public class ErrorReportController : AuthorizedController
    {
        [Route("api/errorreport")]
        [HttpPost]
        public void GetToplist(ErrorReportDto dto)
        {
            var userId = GetUserOrGuestId();

            using (var db = new Db.BgDbContext())
            {
                db.ErrorReports.Add(new ErrorReport()
                {
                    Error = dto.Error,
                    Reporter = db.Users.Single(u => u.Id.ToString() == userId),
                    Reproduce = dto.Reproduce
                });

                db.SaveChanges();
            }
        }
    }
}
