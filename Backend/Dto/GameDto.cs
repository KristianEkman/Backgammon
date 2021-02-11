
namespace Backend.Dto
{
    public class GameDto
    {
        public string id { get; set; }
        public PlayerDto blackPlayer { get; set; }
        public PlayerDto whitePlayer { get; set; }

        public PlayerColor currentPlayer { get; set; }
        public GameState playState { get; set; }
        public PointDto[] points { get; set; }
        public DiceDto[] roll { get; set; }
        public MoveDto[] validMoves { get; set; }
    }
}