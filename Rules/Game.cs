using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//https://www.bkgm.com/rules.html

namespace Backend.Rules
{

    public class Game
    {
        public Guid Id { get; set; }
        public Player BlackPlayer { get; set; }
        public Player WhitePlayer { get; set; }
        public Player.Color CurrentPlayer { get; set; }
        public List<Point> Points { get; set; } = new List<Point>();
        public List<Dice> Roll { get; set; } = new List<Dice>();
        public List<Move> ValidMoves { get; set; } = new List<Move>();
        public State PlayState { get; set; } = State.FirstThrow;
        public DateTime Created { get; set; }
        public DateTime ThinkStart { get; set; }
        public Point[] Bars { get; set; }
        public int GoldMultiplier { get; set; }
        public bool IsGoldGame { get; set; }
        public Player.Color? LastDoubler { get; set; }
        public int Stake { get; set; }

        public Game Clone()
        {
            var game = new Game
            {
                Id = Id,
                BlackPlayer = BlackPlayer.Clone(),
                WhitePlayer = WhitePlayer.Clone(),
                Points = Points.Select(p => p.Clone()).ToList(),
                BlackStarts = BlackStarts,
                WhiteStarts = WhiteStarts,
                Created = Created,
                CurrentPlayer = CurrentPlayer,
                PlayState = PlayState,
                Roll = Roll.Select(r => new Dice { Used = r.Used, Value = r.Value }).ToList(),
                ThinkStart = ThinkStart,
                GoldMultiplier = GoldMultiplier,
                IsGoldGame = IsGoldGame,
                LastDoubler = LastDoubler,
                Stake = Stake,
                Bars = new Point[2]
            };
            game.Bars[(int)Player.Color.Black] = game.Points[0];
            game.Bars[(int)Player.Color.White] = game.Points[25];

            return game;
        }

        public const int ClientCountDown = 40;
        public const int TotalThinkTime = 48;

        public enum State
        {
            OpponentConnectWaiting,
            FirstThrow,
            Playing,
            Ended
        }

        public static Game Create(bool forGold)
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                BlackPlayer = new Player
                {
                    PlayerColor = Player.Color.Black,
                    Name = "Guest"
                },
                WhitePlayer = new Player
                {
                    PlayerColor = Player.Color.White,
                    Name = "Guest"
                },
                Points = new List<Point>(new Point[26]), // 24 points, 1 bar and 1 home,
                Created = DateTime.Now,
                PlayState = State.OpponentConnectWaiting,
                GoldMultiplier = 1,
                IsGoldGame = forGold,
                LastDoubler = null,
            };

            for (int i = 0; i < 26; i++)
            {
                game.Points[i] = new Point
                {
                    BlackNumber = i,
                    WhiteNumber = 25 - i
                };
            }
            game.Bars = new Point[2];
            game.Bars[(int)Player.Color.Black] = game.Points[0];
            game.Bars[(int)Player.Color.White] = game.Points[25];

            game.SetStartPosition();
            game.GoldMultiplier = 1;

            CalcPointsLeft(game);
            return game;
        }

        public void Reset()
        {
            SetStartPosition();
            CalcPointsLeft(this);
            PlayState = State.FirstThrow;
        }

        private static void CalcPointsLeft(Game game)
        {
            var black = 0;
            var white = 0;
            foreach (var point in game.Points)
            {
                foreach (var ckr in point.Checkers.Where(c => c.Color == Player.Color.Black))
                    black += 25 - point.BlackNumber;
                foreach (var ckr in point.Checkers.Where(c => c.Color == Player.Color.White))
                    white += 25 - point.WhiteNumber;
            }
            game.BlackPlayer.PointsLeft = black;
            game.WhitePlayer.PointsLeft = white;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = OtherPlayer();
        }

        public Player.Color OtherPlayer()
        {
            return CurrentPlayer == Player.Color.Black ? Player.Color.White : Player.Color.Black;
        }

        private void SetStartPosition()
        {
            foreach (var point in Points)
                point.Checkers.Clear();

            AddCheckers(2, Player.Color.Black, 1);
            AddCheckers(2, Player.Color.White, 1);

            AddCheckers(5, Player.Color.Black, 12);
            AddCheckers(5, Player.Color.White, 12);

            AddCheckers(3, Player.Color.Black, 17);
            AddCheckers(3, Player.Color.White, 17);

            AddCheckers(5, Player.Color.Black, 19);
            AddCheckers(5, Player.Color.White, 19);

            // CloseToVictory();
            // DebugBar();
            // DebugBlocked();
            // DebugBearingOff();
            // AtHomeAndOtherAtBar();
            // AtHomeAndOtherAtBar2();
            // Test();
            // LegalMove();
        }

        private void Test()
        {
            ClearCheckers();
            AddCheckers(1, Player.Color.Black, 1);
            AddCheckers(1, Player.Color.Black, 2);
            AddCheckers(2, Player.Color.White, 20);

            AddCheckers(5, Player.Color.Black, 21);
            AddCheckers(5, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.Black, 18);

            AddCheckers(3, Player.Color.White, 1);
            AddCheckers(3, Player.Color.White, 5);
        }

        private void AtHomeAndOtherAtBar()
        {
            AddCheckers(3, Player.Color.Black, 21);
            AddCheckers(2, Player.Color.Black, 22);
            AddCheckers(5, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.Black, 24);
            AddCheckers(2, Player.Color.Black, 25);

            AddCheckers(2, Player.Color.White, 19);
            AddCheckers(2, Player.Color.White, 20);
            AddCheckers(3, Player.Color.White, 21);
            AddCheckers(2, Player.Color.White, 22);
            AddCheckers(2, Player.Color.White, 23);
            AddCheckers(1, Player.Color.White, 24);
            AddCheckers(2, Player.Color.White, 0);

        }


        private void AtHomeAndOtherAtBar2()
        {
            AddCheckers(3, Player.Color.Black, 19);
            AddCheckers(3, Player.Color.Black, 20);
            AddCheckers(3, Player.Color.Black, 21);
            AddCheckers(5, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.Black, 24);

            AddCheckers(14, Player.Color.White, 25);
            AddCheckers(1, Player.Color.White, 0);

        }

        private void CloseToVictory()
        {
            //Only one move to victory
            AddCheckers(12, Player.Color.Black, 25);
            AddCheckers(12, Player.Color.White, 25);

            AddCheckers(1, Player.Color.Black, 24);
            AddCheckers(1, Player.Color.White, 24);

            AddCheckers(2, Player.Color.Black, 12);
            AddCheckers(2, Player.Color.White, 12);
        }

        private void DebugBlocked()
        {
            AddCheckers(2, Player.Color.Black, 19);

            AddCheckers(2, Player.Color.Black, 20);
            AddCheckers(3, Player.Color.White, 20);

            AddCheckers(2, Player.Color.Black, 21);
            AddCheckers(3, Player.Color.White, 21);

            AddCheckers(3, Player.Color.Black, 22);
            AddCheckers(3, Player.Color.White, 22);

            AddCheckers(3, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.White, 23);

            AddCheckers(2, Player.Color.Black, 24);
            AddCheckers(2, Player.Color.White, 24);

            AddCheckers(1, Player.Color.Black, 0);
            AddCheckers(1, Player.Color.White, 0);
        }

        private void DebugBar()
        {
            AddCheckers(1, Player.Color.Black, 19);
            AddCheckers(1, Player.Color.White, 19);


            AddCheckers(1, Player.Color.Black, 20);
            AddCheckers(1, Player.Color.White, 20);

            AddCheckers(2, Player.Color.Black, 21);
            AddCheckers(3, Player.Color.White, 21);

            AddCheckers(3, Player.Color.Black, 22);
            AddCheckers(3, Player.Color.White, 22);

            AddCheckers(3, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.White, 23);

            AddCheckers(2, Player.Color.Black, 24);
            AddCheckers(2, Player.Color.White, 24);

            AddCheckers(3, Player.Color.Black, 0);
            AddCheckers(3, Player.Color.White, 0);
        }

        private void DebugBearingOff()
        {
            AddCheckers(3, Player.Color.Black, 20);
            AddCheckers(3, Player.Color.White, 20);

            AddCheckers(3, Player.Color.Black, 21);
            AddCheckers(3, Player.Color.White, 21);

            AddCheckers(3, Player.Color.Black, 22);
            AddCheckers(3, Player.Color.White, 22);

            AddCheckers(3, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.White, 23);

            AddCheckers(2, Player.Color.Black, 24);
            AddCheckers(2, Player.Color.White, 24);

            AddCheckers(1, Player.Color.Black, 19);
            AddCheckers(1, Player.Color.White, 19);
        }

        private void LegalMove()
        {
            AddCheckers(1, Player.Color.White, 0); // 0
            AddCheckers(1, Player.Color.White, 1); // 1
            AddCheckers(2, Player.Color.White, 20);  // 20
            AddCheckers(2, Player.Color.White, 21);  // 21
            AddCheckers(1, Player.Color.White, 22);  // 22
            AddCheckers(4, Player.Color.White, 23);  // 23
            AddCheckers(4, Player.Color.White, 24);  // 24

            AddCheckers(3, Player.Color.Black, 23);
            AddCheckers(3, Player.Color.Black, 22);
            AddCheckers(5, Player.Color.Black, 21);
            AddCheckers(2, Player.Color.Black, 19);
            AddCheckers(2, Player.Color.Black, 17);
            SwitchPlayer();
        }

        public void ClearCheckers()
        {
            foreach (var point in Points)
                point.Checkers.Clear();
        }

        public bool IsBearingOff(Player.Color color)
        {
            // Points that have checkers with the color asked all have higher number than 18
            return Points.Where(p => p.Checkers.Any(p => p.Color == color)).All(p => p.GetNumber(color) >= 19);
        }

        public void AddCheckers(int count, Player.Color color, int point)
        {
            for (int i = 0; i < count; i++)
                Points.Single(p => p.GetNumber(color) == point).Checkers.Add(new Checker { Color = color });
        }

        public void FakeRoll(int v1, int v2)
        {
            Roll = new List<Dice>(Dice.GetDices(v1, v2));
        }

        public int WhiteStarts = 0;
        public int BlackStarts = 0;

        public void SetFirstRollWinner()
        {
            if (this.PlayState == State.FirstThrow)
            {
                if (Roll[0].Value > Roll[1].Value)
                {
                    CurrentPlayer = Player.Color.Black;
                    BlackStarts++;
                }
                else if (Roll[0].Value < Roll[1].Value)
                {
                    CurrentPlayer = Player.Color.White;
                    WhiteStarts++;
                }

                if (Roll[0].Value != Roll[1].Value)
                    PlayState = State.Playing;
            }
        }


        public void RollDice()
        {
            Roll = new List<Dice>(Dice.Roll());
            SetFirstRollWinner();

            ClearMoves(ValidMoves);
            GenerateMoves(ValidMoves);
        }

        public void NewRoll()
        {
            Roll = new List<Dice>(Dice.Roll());
        }

        private void ClearMoves(List<Move> moves)
        {
            // This will probably make it a lot easier for GC, and might even prevent memory leaks.
            foreach (var move in moves)
            {
                if (move.NextMoves != null && move.NextMoves.Any())
                {
                    ClearMoves(move.NextMoves);
                    move.NextMoves.Clear();
                }
            }
            moves.Clear();
        }

        public List<Move> GenerateMoves()
        {
            var moves = new List<Move>();

            GenerateMoves(moves);

            // Making sure both dice are played.
            if (moves.Any(m => m.NextMoves.Any()))
            {
                moves = moves.Where(m => m.NextMoves.Any()).ToList();
            }
            else if (moves.Any())
            {
                // All moves have zero next move in this block.
                // Only one dice can be use and it must be the one with highest value
                var first = moves.OrderByDescending(m => m.To.GetNumber(CurrentPlayer) - m.From.GetNumber(CurrentPlayer)).First();
                moves.Clear();
                moves.Add(first);
            }
            return moves;
        }

        public Point GetHome(Player.Color color)
        {
            return Points.Single(p => p.GetNumber(color) == 25);
        }

        public static (int black, int white) CalcPointsLeft2(Game game)
        {
            var black = 0;
            var white = 0;
            foreach (var point in game.Points)
            {
                foreach (var ckr in point.Checkers.Where(c => c.Color == Player.Color.Black))
                    black += 25 - point.BlackNumber;
                foreach (var ckr in point.Checkers.Where(c => c.Color == Player.Color.White))
                    white += 25 - point.WhiteNumber;
            }
            return (black, white);
        }

        private void GenerateMoves(List<Move> moves)
        {
            var bar = Points.Where(p => p.GetNumber(CurrentPlayer) == 0);
            var barHasCheckers = bar.First().Checkers.Any(c => c.Color == CurrentPlayer);

            foreach (var dice in Roll.OrderByDescending(r => r.Value))
            {
                if (dice.Used)
                    continue;
                dice.Used = true;

                var points = barHasCheckers ? bar :
                    Points.Where(p => p.Checkers.Any(c => c.Color == CurrentPlayer))
                    .OrderBy(p => p.GetNumber(CurrentPlayer));

                foreach (var fromPoint in points)
                {
                    var fromPointNo = fromPoint.GetNumber(CurrentPlayer);
                    if (fromPointNo == 25)
                        continue;
                    var toPoint = Points.SingleOrDefault(p => p.GetNumber(CurrentPlayer) == dice.Value + fromPointNo);

                    if (toPoint != null && toPoint.IsOpen(CurrentPlayer) && !moves.Any(m => m.From == fromPoint && m.To == toPoint)
                        && !toPoint.IsHome(CurrentPlayer)) // no creation of bearing off moves here. See next block.
                    {
                        var move = new Move { Color = CurrentPlayer, From = fromPoint, To = toPoint };
                        moves.Add(move);
                        var hit = MakeMove(move);
                        GenerateMoves(move.NextMoves);
                        UndoMove(move, hit);
                    }

                    if (IsBearingOff(CurrentPlayer))
                    {
                        // The furthest away checker can be moved beyond home.
                        var minPoint = Points.Where(p => p.Checkers.Any(c => c.Color == CurrentPlayer)).OrderBy(p => p.GetNumber(CurrentPlayer)).First().GetNumber(CurrentPlayer);
                        var toPointNo = fromPointNo == minPoint ? Math.Min(25, fromPointNo + dice.Value) : fromPointNo + dice.Value;
                        toPoint = Points.SingleOrDefault(p => p.GetNumber(CurrentPlayer) == toPointNo);
                        if (toPoint != null && toPoint.IsOpen(CurrentPlayer) && !moves.Any(m => m.From == fromPoint && m.To == toPoint))
                        {
                            var move = new Move { Color = CurrentPlayer, From = fromPoint, To = toPoint };
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

        public Checker MakeMove(Move move)
        {
            var checker = move.From.Checkers.LastOrDefault(c => c.Color == move.Color);
            if (checker == null)
                throw new ApplicationException("There should be a checker on this point. Something is very wrong.");
            move.From.Checkers.Remove(checker);
            move.To.Checkers.Add(checker);
            if (move.Color == Player.Color.Black)
            {
                BlackPlayer.PointsLeft -= (move.To.BlackNumber - move.From.BlackNumber);
            }
            else
            {
                WhitePlayer.PointsLeft -= (move.To.WhiteNumber - move.From.WhiteNumber);
            }

            // Feels wrong that now that own home is same point as opponent bar.
            // Todo: Try to change it some day.
            var hit = move.To.IsHome(move.Color) ? null : move.To.Checkers.SingleOrDefault(c => c.Color != checker.Color);
            if (hit != null)
            {
                move.To.Checkers.Remove(hit);
                var bar = Points.Single(p => p.GetNumber(OtherPlayer()) == 0);
                bar.Checkers.Add(hit);
                if (move.Color == Player.Color.Black)
                    WhitePlayer.PointsLeft += (move.To.WhiteNumber);
                else
                    BlackPlayer.PointsLeft += (move.To.BlackNumber);
            }
            //AssertPointsLeft();
            return hit;
        }

        private void AssertPointsLeft()
        {
            var (black, white) = CalcPointsLeft2(this);
            if (black != BlackPlayer.PointsLeft)
                Debugger.Break();
            if (white != WhitePlayer.PointsLeft)
                Debugger.Break();
        }

        public void UndoMove(Move move, Checker hitChecker)
        {
            var checker = move.To.Checkers.LastOrDefault(c => c.Color == move.Color);
            move.To.Checkers.Remove(checker);
            move.From.Checkers.Add(checker);
            if (move.Color == Player.Color.Black)
                BlackPlayer.PointsLeft += (move.To.BlackNumber - move.From.BlackNumber);
            else
                WhitePlayer.PointsLeft += (move.To.WhiteNumber - move.From.WhiteNumber);

            if (hitChecker != null)
            {
                move.To.Checkers.Add(hitChecker);
                var bar = Points.Single(p => p.GetNumber(OtherPlayer()) == 0);
                bar.Checkers.Remove(hitChecker);
                if (move.Color == Player.Color.Black)
                    WhitePlayer.PointsLeft -= (move.To.WhiteNumber);
                else
                    BlackPlayer.PointsLeft -= (move.To.BlackNumber);
            }
            //AssertPointsLeft();
        }

        public bool PlayersPassed()
        {
            int lastBlack = 0;
            int lastWhite = 0;

            for (int i = 0; i < 25; i++)
            {
                if (Points[i].Checkers.Any(p => p.Color == Player.Color.Black))
                {
                    lastBlack = Points[i].GetNumber(Player.Color.Black);
                    break;
                }
            }


            for (int i = 25 - 1; i >= 1; i--)
            {
                if (Points[i].Checkers.Any(p => p.Color == Player.Color.White))
                {
                    lastWhite = Points[i].GetNumber(Player.Color.Black);
                    break;
                }
            }
            return lastBlack > lastWhite;
        }

        public bool ReallyStarted()
        {
            return BlackPlayer.FirstMoveMade && WhitePlayer.FirstMoveMade;
        }

        public string GameString()
        {
            var s = new StringBuilder("board ");

            var blackBar = Points[0].Checkers.Count(c => c.Color == Player.Color.Black);
            s.Append($"b{blackBar} ");

            for (int i = 1; i < 25; i++)
            {
                var checkers = Points[i].Checkers;
                if (checkers.Count > 0)
                {
                    var color = checkers[0].Color;
                    if (color == Player.Color.Black)
                        s.Append('b');
                    else
                        s.Append('w');
                }
                s.Append(checkers.Count + " ");
            }
            var whiteBar = Points[25].Checkers.Count(c => c.Color == Player.Color.White);
            s.Append($"w{whiteBar} ");

            var whiteHome = Points[0].Checkers.Count(c => c.Color == Player.Color.White);
            s.Append($"{whiteHome} ");

            var blackHome = Points[25].Checkers.Count(c => c.Color == Player.Color.Black);
            s.Append($"{blackHome} ");

            s.Append(CurrentPlayer == Player.Color.Black ? "b " : "w ");
            s.Append(Roll[0].Value + " ");
            s.Append(Roll[1].Value);
            return s.ToString();
        }
    }
}
