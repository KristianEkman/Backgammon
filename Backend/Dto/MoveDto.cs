//https://www.bkgm.com/rules.html

namespace Backend.Dto
{
    public class MoveDto
    {
        public PlayerColor Color { get; set; }
        public int From { get; set; }
        public MoveDto[] NextMoves { get; set; }
        public int To { get; set; }
    }
}