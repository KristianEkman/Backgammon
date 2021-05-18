using Backend.Db;
using Backend.Dto.message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    public class MessageController : AuthorizedController
    {
        private readonly ILogger<MessageController> logger;
        public MessageController(ILogger<MessageController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("api/message/users")]
        public MessageDto[] GetMessages()
        {
            using (var db = new BgDbContext())
            {
                var user = GetUser(db);                
                var ms = db.Messages.Where(m => m.Receiver == user).Select(m => new MessageDto
                {
                    Text = m.Text,
                    Type = m.Type,
                    Id = m.Id,
                    Sender = m.Sender.Name,
                    Date = m.Sent.ToString("dd MMM, yy")
                }).ToArray();
                return ms;
            }
        }

        [HttpPut]
        [Route("api/message/addallsharepromptmessages")]
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

        [HttpDelete]
        [Route("api/message/delete")]
        public void Delete(int id)
        {
            using (var db = new BgDbContext())
            {
                var user = GetUser(db);
                var message = db.Messages.Single(m => m.Id == id);
                if (message.Receiver != user)
                    throw new UnauthorizedAccessException("Not allowed to delete that message.");
                user.ReceivedMessages.Remove(message);
                db.SaveChanges();
            }
        }

        [HttpPost]
        [Route("api/message/sendToAll")]
        public void SendToAll(Dto.message.MessageType type)
        {
            // For each user, add the message. If the user has notification flag, send mail.
            AssertAdmin();
            string adminId = Request.Headers["user-id"].ToString();
            using (var db = new BgDbContext())
            {
                var admin = db.Users.First(u => u.Id.ToString() == adminId);               
                foreach (var user in db.Users.Where(u => u.Admin))
                {
                    (string Subject, string Text) st = GetSubjectAndText(type, user.EmailUnsubscribeId, user.Name);
                    var subject = st.Subject;
                    var text = st.Text;
                    user.ReceivedMessages.Add(new Message
                    {
                        Text = "",
                        Type = type,
                        Sender = admin,
                        Sent = DateTime.Now
                    });

                    if (user.EmailNotifications && !string.IsNullOrWhiteSpace(user.Email))
                    {
                        try
                        {
                            Mail.Mailer.Send(user.Email, subject, text);
                        }
                        catch (Exception exc)
                        {
                            logger.LogError(exc.ToString());
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        private (string, string) GetSubjectAndText(MessageType type, Guid unsbsciberId, string name)
        {
            switch (type)
            {
                case MessageType.SharePrompt:
                    return ("", "");
                case MessageType.Version2Info:
                    return ("New version of Backgammon", @$"<p>Hi {name}!</p>
<img src='https://backgammon.azurewebsites.net/assets/images/banner.jpg'>
<p>You have a new Backgammon message.</p>
<p><a href='https://backgammon.azurewebsites.net/messages'>Go there and read the news.<a/></p>
<p><a href='https://backgammon.azurewebsites.net/message/unsubscribe?id={unsbsciberId}'>Unsubscribe from all email notifications.</a></p>
<p>
    Kind Regards<br/>
    /Kristian
</p>
");
                default:
                    throw new NotImplementedException();
            }
        }

        [HttpGet]
        [Route("api/message/unsubscribe")]
        public async void Unsubscribe(Guid id)
        {
            using (var db = new BgDbContext())
            {
                var user = db.Users.Single(u => u.EmailUnsubscribeId == id);
                user.EmailNotifications = false;
                db.SaveChanges();
            }

            await Response.WriteAsync("You will not receive any more notifications.");
        }
    }
}
