using Backend.Dto.message;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class Message
    {
        public int Id { get; set; }        
        public Dto.message.MessageType Type { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime Sent { get; set; }
        public string Text { get; set; }
        public User Receiver { get; set; }
        public User Sender { get; set; }
    }
}
