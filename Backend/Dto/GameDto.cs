
namespace Backend.Dto
{
    public class GameDto
    {
        public string Id { get; set; }
        public PlayerDto BlackPlayer { get; set; }
        public PlayerDto WhitePlayer { get; set; }

        public PlayerColor CurrentPlayer { get; set; }
        public GameState PlayState { get; set; }
        public PointDto[] Points { get; set; }
        public DiceDto[] Roll { get; set; }
        public MoveDto[] ValidMoves { get; set; }
    }
}