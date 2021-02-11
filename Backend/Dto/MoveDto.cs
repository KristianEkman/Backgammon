//https://www.bkgm.com/rules.html

namespace Backend.Dto
{
    public class MoveDto
    {
        public PlayerColor color { get; set; }
        public int from { get; set; }
        public MoveDto[] nextMoves { get; set; }
        public int to { get; set; }
    }
}