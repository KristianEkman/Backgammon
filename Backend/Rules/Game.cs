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
        public Dice[] Roll { get; set; }
        public List<Move> ValidMoves { get; set; } = new List<Move>();

        public enum State
        {
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

            // The bar, is shared 
            game.Points[0] = new Point();
            game.Points[0].BlackNumber = 0;
            game.Points[0].WhiteNumber = 0;

            for (int i = 1; i < 25; i++)
            {
                game.Points[i] = new Point();
                game.Points[i].BlackNumber = i;
                game.Points[i].WhiteNumber = 25 - i;
            }

            for (int i = 0; i < 15; i++)
            {
                game.BlackPlayer.Checkers.Add(new Checker { Color = Player.Color.Black });
                game.WhitePlayer.Checkers.Add(new Checker { Color = Player.Color.White });
            }

            game.SetStartPosition();

            return game;
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

        public void FakeRoll(int v1, int v2)
        {
            Roll = new Dice[]
            {
                new Dice{Value = v1},
                new Dice{Value = v2}
            };
        }

        public void RollDice()
        {
            Roll = Dice.Roll();
            ClearMoves(ValidMoves);
            GenerateMoves(ValidMoves);            
        }

        private void ClearMoves(List<Move> moves)
        {
            // This will probably make it alot easier for GC, and might even prevent memory leaks.
            foreach (var move in moves)
            {
                if (move.NextMoves != null && move.NextMoves.Any())
                {
                    ClearMoves(move.NextMoves);
                    move.NextMoves.Clear();
                }
            }
        }

        public void GenerateMoves(List<Move> moves)
        {
            foreach (var dice in Roll)
            {
                if (dice.Used)
                    continue;
                dice.Used = true;
                foreach (var point in Points)
                {
                    if (point.Checkers.Any(c => c.Color == CurrentPlayer))
                    {
                        var fromPointNo = point.GetNumber(CurrentPlayer);
                        var toPoint = Points.SingleOrDefault(p => p.GetNumber(CurrentPlayer) == dice.Value + fromPointNo);
                        
                        if (toPoint != null && toPoint.IsOpen(CurrentPlayer))
                        {
                            var move = new Move { Color = CurrentPlayer, From = point, To = toPoint};
                            moves.Add(move);
                            var hit = MakeMove(move);
                            GenerateMoves(move.NextMoves);
                            UndoMove(move, hit);
                        }
                    }
                }
                dice.Used = false;
            }
        }

        private Checker MakeMove(Move move)
        {
            var checker = move.From.Checkers.FirstOrDefault();
            if (checker == null)
                throw new ApplicationException("There should be a checker on this point. Something is very wrong.");
            move.From.Checkers.Remove(checker);
            move.To.Checkers.Add(checker);
            var hit = move.To.Checkers.SingleOrDefault(c => c.Color != checker.Color);
            if (hit != null)
            {
                move.To.Checkers.Remove(hit);
                Points[0].Checkers.Add(hit);
            }
            return hit;
        }

        private void UndoMove(Move move, Checker hitChecker)
        {
            var checker = move.To.Checkers.FirstOrDefault();
            move.To.Checkers.Remove(checker);
            move.From.Checkers.Add(checker);
            if (hitChecker != null)
            {
                move.To.Checkers.Add(hitChecker);
                Points[0].Checkers.Remove(hitChecker);
            }
        }



    }
}
