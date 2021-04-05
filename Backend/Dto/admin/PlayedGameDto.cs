using System;

namespace Backend.Dto.admin
{
    public class PlayedGameDto
    {
        public DateTime started { get; set; }
        public string black { get; set; }
        public string white { get; set; }
        public PlayerColor? winner { get; set; }
    }
}