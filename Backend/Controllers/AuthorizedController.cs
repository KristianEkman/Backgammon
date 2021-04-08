using Backend.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Backend.Controllers
{
    public class AuthorizedController : ControllerBase
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
                    throw new UnauthorizedAccessException();
            }
            return userId;
        }

        protected User GetUser(BgDbContext db)
        {
            var userId = Request.Headers["user-id"].ToString();
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException();

            return db.Users.Single(u => u.Id.ToString().Equals(userId));
        }

        protected string GetUserOrGuestId()
        {
            var userId = Request.Headers["user-id"].ToString();
            if (string.IsNullOrWhiteSpace(userId))
                return Guid.Empty.ToString();

            return userId;
        }

        protected void AssertAdmin()
        {
            var userId = Request.Headers["user-id"].ToString();
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException();

            using (var db = new Db.BgDbContext())
            {
                var user = db.Users.SingleOrDefault(u => u.Id.ToString() == userId);
                if (user == null || !user.Admin)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
