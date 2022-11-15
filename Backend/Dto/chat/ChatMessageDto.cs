namespace Backend.Dto.chat
{
    public class ChatMessageDto : ChatDto
    {
        public string fromUser { get; set; }
        public string message { get; set; }
    }
}
