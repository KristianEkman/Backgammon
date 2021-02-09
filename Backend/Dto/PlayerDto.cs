namespace Backend.Dto
{
    public class PlayerDto
    {
        public PlayerColor PlayerColor { get; set; }
    }

    public enum PlayerColor
    {
        Black,
        White
    }
}