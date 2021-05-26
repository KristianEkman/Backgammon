namespace Backend.Dto
{
    public class PlayerDto
    {
        public string name { get; set; }

        public PlayerColor playerColor { get; set; }

        public int pointsLeft { get; set; }
        public string photoUrl { get; set; }
        public int elo { get; set; }
        public int gold { get; set; }

    }
}