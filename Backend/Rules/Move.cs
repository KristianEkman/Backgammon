//https://www.bkgm.com/rules.html

namespace Backend.Rules
{
    public class Move
    {
        public Point From { get; set; }
        public Point To { get; set; }
        public Player.Color Color { get; set; }
    }
}
