using Backend.Db;
using Backend.Dto.feedback;
using Backend.Dto.message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IConfiguration config;
        public FeedbackController(ILogger<MessageController> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.config = configuration;
        }

        [HttpGet]
        [Route("api/feedback/posts")]
        public FeedbackDto[] GetFeedBack(int skip)
        {
            using (var db = new BgDbContext())
            {
                var user = GetUser(db);
                var ms = db.Feedback.Skip(skip).Select(m => new FeedbackDto
                {
                    Text = m.Text,
                    Id = m.Id,
                    SenderName = m.Sender.Name,
                    Sent = m.Sent.ToString("dd MMM, yy")
                }).ToArray();
                return ms;
            }
        }

        [HttpPut]
        [Route("api/feedback/addallsharepromptmessages")]
        public void AddAllSharePromptMessages()
        {
            AssertAdmin();
            string adminId = Request.Headers["user-id"].ToString();

            using (var db = new BgDbContext())
            {
                var admin = db.Users.First(u => u.Id.ToString() == adminId);
                foreach (var user in db.Users)
                {
                    // Do not waste space in db for a generic message.
                    user.ReceivedMessages.Add(new Message
                    {
                        Text = "",
                        Type = MessageType.SharePrompt,
                        Sender = admin,
                        Sent = DateTime.Now
                    });
                }
                db.SaveChanges();
            }
        }
    }
}
