using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibsIntegration
{
    public class Board
    {
        public string PlayerName { get; set; }
        public string OpponentName { get; set; }
        public int MatchLength { get; set; }
        public int PlayerGot { get; set; }
        public int OpponentGot { get; set; }
        // Positive numbers represent O's pieces negative numbers represent X's pieces
        public int[] Checkers { get; } = new int[26];
        //-1 if it's X's turn, +1 if it's O's turn 0 if the game is over
        public Players? Turn { get; set; }
        public int[] PlayerDice { get; } = new int[2];
        public int[] OpponentDice { get; } = new int[2];
        public int Cube { get; set; }
        public int PlayerMayDouble { get; set; }
        public int OpponentMayDouble { get; set; }
        public bool WasDoubled { get; set; }
        //-1 if you are X, +1 if you are O
        public Players YourColor { get; set; }
        //-1 if you play from position 24 to position 1 +1 if you play from position 1 to position 24
        public bool ReverseDirection { get; set; }
        //0 or 25, obsolete
        public int HomeIndex { get; set; }
               

        //0 or 25, obsolete
        public int BarIndex { get; set; }
        public int PlayerHome { get; set; }
        public int OpponentHome { get; set; }
        public int PlayerBar { get; set; }
        public int OpponentBar { get; set; }
        public int CanMove { get; set; }
        //don't use this token
        public int ForcedMove { get; set; }
        //don't use this token
        public int DidCrawford { get; set; }
        //maximum number of instant redoubles in unlimited matches
        public int Redoubles { get; set; }

        public static Board Parse(string rawBoard)
        {
            var split = rawBoard.Replace(">", "").Trim().Split(":");
            var b = new Board();
            int i = 0;
            if (split[i++] != "board")
                throw new ApplicationException("Rawboard should start with board");
            b.PlayerName = split[i++];
            b.OpponentName = split[i++];
            b.MatchLength = int.Parse(split[i++]);
            b.PlayerGot = int.Parse(split[i++]);
            b.OpponentGot = int.Parse(split[i++]);
            for (int c = 0; c < 26; c++)
                b.Checkers[c] = int.Parse(split[c + i]);
            i += 26;
            b.Turn = split[i++] == "1" ? Players.Player : Players.Opponent;
            b.PlayerDice[0] = int.Parse(split[i++]);
            b.PlayerDice[1] = int.Parse(split[i++]);
            b.OpponentDice[0] = int.Parse(split[i++]);
            b.OpponentDice[1] = int.Parse(split[i++]);
            b.Cube = int.Parse(split[i++]);
            b.PlayerMayDouble = int.Parse(split[i++]);
            b.OpponentMayDouble = int.Parse(split[i++]);
            b.WasDoubled = split[i++] == "1";
            b.YourColor = split[i++] == "1" ? Players.Player : Players.Opponent;
            b.ReverseDirection = split[i++] == "-1";            
            b.HomeIndex = int.Parse(split[i++]);
            b.BarIndex = int.Parse(split[i++]);
            b.PlayerHome = int.Parse(split[i++]);
            b.OpponentHome = int.Parse(split[i++]);
            b.PlayerBar = int.Parse(split[i++]);
            b.OpponentBar = int.Parse(split[i++]);
            b.CanMove = int.Parse(split[i++]);
            b.ForcedMove = int.Parse(split[i++]);
            b.DidCrawford = int.Parse(split[i++]);
            b.Redoubles = int.Parse(split[i++]);
            return b;
        }

        public string ToInternal()
        {
            // 31 tokens
            // BlackBar, pos 1 - 24, WhiteBar, BlackHome, WhiteHome, turn, dice1, dice2
            // "0 b2 0 0 0 0 w5 0 w3 0 0 0 b5 w5 0 0 0 b3 0 b5 0 0 0 0 w2 0 0 0 b 5 6"
            // Convention Player is always black, Opponent is always white

            //Antar add black alltid spelar framåt.
            if (YourColor != Turn)
            {
                throw new ApplicationException("Not calculating opponnets boards");
            }
            var s = "board ";
            for (int i = 0; i < 26; i++)
            {
                //int r = ReverseDirection ? 25 - i : i;
                int r = i;
                if (Checkers[r] < 0)
                    s += "b";
                else if (Checkers[r] > 0)
                    s += "w";
                s += Math.Abs(Checkers[r]);
                s += " ";
            };

            s += Math.Abs(PlayerHome);
            s += " ";
            s += Math.Abs(OpponentHome);
            s += " ";
            s += ReverseDirection ? "w " : "b ";
            s += $"{PlayerDice[0]} ";
            s += $"{PlayerDice[1]}";

            return s;
        }
    }

    public enum Players
    {
        //X
        Player,
        //O
        Opponent
    }
}
