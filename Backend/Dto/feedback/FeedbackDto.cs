using Backend.Db;
using System;

namespace Backend.Dto.feedback
{
    public class FeedbackDto
    {
        public int Id { get; set; }

        public string Sent { get; set; }

        public string Text { get; set; }

        public string SenderName { get; set; }

        //public int ReplyTo { get; set; }
    }
}
