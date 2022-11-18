using System;
namespace Backend.Dto.chat
{
	public class UserTypingDto : ChatDto
	{
		public UserTypingDto()
		{
		}

        public string message { get; set; }
        public string user { get; set; }
    }
}
