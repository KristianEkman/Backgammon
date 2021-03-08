
using System;

namespace Backend.Dto
{
    public class GameDto
    {

        public string id { get; set; }
        public PlayerDto blackPlayer { get; set; }
        public PlayerDto whitePlayer { get; set; }
        public PlayerColor currentPlayer { get; set; }
        public PlayerColor winner { get; set; } = PlayerColor.neither;
        public GameState playState { get; set; }
        public PointDto[] points { get; set; }
        public MoveDto[] validMoves { get; set; }
        public double thinkTime { get; set; }
    }
}