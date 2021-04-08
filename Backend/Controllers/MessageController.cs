using Backend.Db;
using Backend.Dto.message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    public class MessageController : AuthorizedController
    {
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
                    // todo: Add this when a user is registered.
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
    }
}
