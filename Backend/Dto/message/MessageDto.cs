using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Dto.message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
        public MessageType Type { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
    }
}
