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
        public int[] Roll { get; set; }
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
            var roll = Dice.Roll();
            if (roll.Item1 == roll.Item2)
            {
                Roll = new int[] { roll.Item1, roll.Item1, roll.Item1, roll.Item1 };
            }
            else
            {
                Roll = (new int[] { roll.Item1, roll.Item2 }).OrderByDescending(r => r).ToArray();
            }

            ValidMoves.Clear();
            GenerateMoves(0, ValidMoves);            
            // todo: disallow single moves when there is an option to play both moves.
        }

        public void GenerateMoves(int diceIndex, List<Move> moves)
        {
            CollectMoves(diceIndex, moves);
            foreach (var move in moves)
            {
                var hitChecker = MakeMove(move);
                if (diceIndex < Roll.Length - 1)
                    GenerateMoves(diceIndex + 1, move.NextMoves);
                else
                {
                    // Todo: Evaluate to give a score.
                }
                UndoMove(move, hitChecker);
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


        /// <summary>
        /// Collects all moves for one dice in a list.
        /// </summary>        
        private void CollectMoves(int rollIndex, List<Move> moves)
        {
            // Calculate all posiible move sets.
            // One move affects what other moves can be made, so the moves should perhaps be treated as a set?
            
            // todo: If there is a checker on the bar, it must be moved first.
            var player = CurrentPlayer == Player.Color.Black ? BlackPlayer : WhitePlayer;
            var roll = Roll[rollIndex];
            foreach (var point in Points)
            {
                var pointNumber = CurrentPlayer == Player.Color.Black ? point.BlackNumber : point.WhiteNumber;
                if (point.Checkers.Any(checker => checker.Color == CurrentPlayer))
                {
                    var toPointNumber = roll + pointNumber;
                    var toPoint = Points.SingleOrDefault(p => p.GetNumber(CurrentPlayer) == toPointNumber);
                    if (toPoint != null && toPoint.IsOpen(CurrentPlayer))
                    {
                        moves.Add(new Move { Color = CurrentPlayer, From = point, To = toPoint });
                        continue;
                    }
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
