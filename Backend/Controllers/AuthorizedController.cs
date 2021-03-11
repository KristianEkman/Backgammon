using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Backend.Controllers
{
    public abstract class AuthorizedController : ControllerBase
    {
        protected string GetUserId()
        {
            var userId = Request.Headers["user-id"].ToString();
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException();

            using (var db = new Db.BgDbContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id.ToString() == userId);
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            return userId;
        }
    }
}
