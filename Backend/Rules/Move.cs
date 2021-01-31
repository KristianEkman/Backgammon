//https://www.bkgm.com/rules.html

using System.Collections.Generic;

namespace Backend.Rules
{
    public class Move
    {
        public Point From { get; set; }
        public Point To { get; set; }
        public Player.Color Color { get; set; }

        public List<Move> NextMoves { get; set; } = new List<Move>();
    }
}
