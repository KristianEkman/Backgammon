using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//https://www.bkgm.com/rules.html

namespace Backend.Rules
{

    public class Game
    {
        public Player BlackPlayer { get; set; }
        public Player WhitePlayer { get; set; }
        public Player.Color CurrentPlayer { get; set; }
        public List<Point> Points { get; set; }
        public Nullable<(int, int)> Roll { get; set; }
        public List<Move> ValidMoves { get; set; } = new List<Move>();

        public enum State {
            FirstThrow,
            Playing,
            Ended
        }

        public static Game Create()
        {
            var game = new Game
            {
                BlackPlayer = new Player
                {
                    PlayerColor = Player.Color.Black,
                },
                WhitePlayer = new Player
                {
                    PlayerColor = Player.Color.White
                },
                Points = new List<Point>(new Point[25])
            };

            for (int i = 0; i < 25; i++)
            {
                game.Points[i] = new Point();
                game.Points[i].BlackNumber = i;
                game.Points[i].WhiteNumber = 24 - i;
            }

            for (int i = 0; i < 15; i++)
            {
                game.BlackPlayer.Checkers.Add(new Checker { Color = Player.Color.Black });
                game.WhitePlayer.Checkers.Add(new Checker { Color = Player.Color.White });
            }

            game.SetStartPosition();
            
            return game;
        }

        public void RollDice()
        {
            Roll = Dice.Roll();
            

            ValidMoves.Clear();
            var player = CurrentPlayer == Player.Color.Black ? BlackPlayer : WhitePlayer;
            foreach (var point in Points)
            {
                var pointNumber = CurrentPlayer == Player.Color.Black ? point.BlackNumber : point.WhiteNumber;
                if (point.Checkers.Any(checker => checker.Color == CurrentPlayer)) {

                }
            }

        }

        private void SetStartPosition()
        {
            int checkerIdx = 0;
            // six point
            for (int i = 0; i < 5; i++)
            {
                Points.Single(p => p.BlackNumber == 6).Checkers.Add(BlackPlayer.Checkers[checkerIdx]);
                Points.Single(p => p.WhiteNumber == 6).Checkers.Add(WhitePlayer.Checkers[checkerIdx]);
                checkerIdx++;
            }

            for (int i = 0; i < 3; i++)
            {
                Points.Single(p => p.BlackNumber == 8).Checkers.Add(BlackPlayer.Checkers[checkerIdx]);
                Points.Single(p => p.WhiteNumber == 8).Checkers.Add(WhitePlayer.Checkers[checkerIdx]);
                checkerIdx++;
            }

            for (int i = 0; i < 5; i++)
            {
                Points.Single(p => p.BlackNumber == 13).Checkers.Add(BlackPlayer.Checkers[checkerIdx]);
                Points.Single(p => p.WhiteNumber == 13).Checkers.Add(WhitePlayer.Checkers[checkerIdx]);
                checkerIdx++;
            }

            for (int i = 0; i < 2; i++)
            {
                Points.Single(p => p.BlackNumber == 24).Checkers.Add(BlackPlayer.Checkers[checkerIdx]);
                Points.Single(p => p.WhiteNumber == 24).Checkers.Add(WhitePlayer.Checkers[checkerIdx]);
                checkerIdx++;
            }

        }

    }
}
