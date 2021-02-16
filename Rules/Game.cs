﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                Id = Guid.NewGuid(),
                BlackPlayer = new Player
                {
                    PlayerColor = Player.Color.Black,
                },
                WhitePlayer = new Player
                {
                    PlayerColor = Player.Color.White
                },
                Points = new List<Point>(new Point[26]) // 24 points, 1 bar and 1 home
            };

            for (int i = 0; i < 26; i++)
            {
                game.Points[i] = new Point();
                game.Points[i].BlackNumber = i;
                game.Points[i].WhiteNumber = 25 - i;
            }

            game.SetStartPosition();

            return game;
        }
        

        private Player.Color OtherPlayer()
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


            //AddCheckers(13, Player.Color.Black, 25);
            //AddCheckers(13, Player.Color.White, 25);

            //AddCheckers(2, Player.Color.Black, 24);
            //AddCheckers(2, Player.Color.White, 24);
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
            SetState();
        }

        private void SetState()
        {
            if (PlayState == State.FirstThrow)
            {
                if (Roll[0].Value > Roll[1].Value)
                    CurrentPlayer = Player.Color.Black;
                else if (Roll[0].Value < Roll[1].Value)
                    CurrentPlayer = Player.Color.White;

                if (Roll[0].Value != Roll[1].Value)
                    PlayState = State.Playing;
            }
        }

        public void RollDice()
        {
            Roll = new List<Dice>(Dice.Roll());
            SetState();
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
            var checker = move.From.Checkers.FirstOrDefault();
            if (checker == null)
                throw new ApplicationException("There should be a checker on this point. Something is very wrong.");
            move.From.Checkers.Remove(checker);
            move.To.Checkers.Add(checker);
            var hit = move.To.Checkers.SingleOrDefault(c => c.Color != checker.Color);
            if (hit != null)
            {
                move.To.Checkers.Remove(hit);
                var bar = Points.Single(p => p.GetNumber(OtherPlayer()) == 0);
                bar.Checkers.Add(hit);
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
                var bar = Points.Single(p => p.GetNumber(OtherPlayer()) == 0);
                bar.Checkers.Remove(hitChecker);
            }
        }

    }
}
