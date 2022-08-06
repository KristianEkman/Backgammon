using Backend.Db;
using Backend.Dto.feedback;
using Backend.Dto.message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    public class FeedbackController : AuthorizedController
    {
        private readonly ILogger<MessageController> logger;
        public FeedbackController(ILogger<MessageController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("api/feedback")]
        public FeedbackDto[] GetFeedBack(int skip)
        {
            using var db = new BgDbContext();
            var ms = db.Feedback.Include(f => f.Sender)
                .OrderByDescending(f => f.PostTime)
                .Skip(skip).Take(10)
                .ToArray()
                .Select(m => new FeedbackDto
                {
                    Text = m.Text,
                    Id = m.Id,
                    SenderName = m.Sender.Name,
                    Sent = m.PostTime.ToString("MMMM dd, yyyy")
                }).ToArray();
            return ms;
        }

        [HttpPost]
        [Route("api/feedback")]
        public void Post(PostFeedbackDto feedbackDto)
        {
            var feedback = feedbackDto.text;
            using var db = new BgDbContext();
            var user = GetUser(db);
            if (user == null)
            {
                logger.LogError("Guest is trying to post feedback.");
                // return silently incase of an attack to minimize resource use.
                // client takes care of validation
                return;
            }

            var today = DateTime.Now.Date;
            var userId = user.Id;
            var count = db.Feedback.Where(f => f.PostTime.Date == today && f.Sender.Id == userId).Count();
            if (count > 50 || feedback.Length > 200)
            {
                logger.LogError("User is trying to post to much feedback.");
                // return silently incase of an attack to minimize resource use.
                // client takes care of validation
                return;
            }

            var dto = new Feedback
            {
                Sender = user,
                PostTime = DateTime.Now,
                Text = feedback,
            };

            db.Feedback.Add(dto);
            db.SaveChanges();
        }

    }
}
