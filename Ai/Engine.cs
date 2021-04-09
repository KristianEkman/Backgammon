using Backend.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ai
{
    public class Engine
    {
        public Engine(Game game)
        {
            Game = game;
            Configuration = new Config();
        }

        public Engine(Game game, Config config)
        {
            Configuration = config;
        }

        private Game Game { get; }

        public Config Configuration { get; }

        public Move[] GetBestMoves()
        {
            Move[] bestMoveSequence = null;
            var bestScore = double.MinValue;
            var allSequences = GenerateMovesSequence();
            //return allSequences[0].ToArray();
            //int depth = Game.Roll.Count == 2 ? 0 : 0;
            var myColor = Game.CurrentPlayer;
            var oponent = Game.OtherPlayer();
            for (int s = 0; s < allSequences.Count; s++)
            {
                var sequence = allSequences[s];
                var hits = DoSequence(sequence);
                var score = EvaluatePoints(myColor) + EvaluateCheckers(myColor);
                //if (bestScore - score < 2) //only eval costly propability score if it is interesting
                if (Configuration.PropabilityScore)
                    score -= PropabilityScore(oponent) * Configuration.PropabilityFactor; // the possibility for the other player to score good.
                //var score = AlpaBeta(int.MinValue, int.MaxValue, depth);
                UndoSequence(sequence, hits);

                //Console.WriteLine($"Engine search {s} of {allSequences.Count}\t{score.ToString("0.##")}\t{sequence.BuildString()}");

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoveSequence = sequence;
                }

            }
            if (bestMoveSequence == null)
                return new Move[0];
            return bestMoveSequence.ToArray();
        }

        private double AlpaBeta(double alpha, double beta, int depth)
        {
            if (depth == 0)
                return PropabilityScore(Game.CurrentPlayer);

            var bestScore = double.MinValue;
            var seqs = GenerateMovesSequence();

            foreach (var seq in seqs)
            {
                var hits = DoSequence(seq);
                var score = -AlpaBeta(-beta, -alpha, depth - 1);
                UndoSequence(seq, hits);

                if (score > bestScore)
                {
                    bestScore = score;
                    if (score > alpha)
                    {
                        if (score >= beta)
                        {
                            // todo: store pv move in hash table
                            return beta;
                        }
                        alpha = score;
                    }
                }
            }

            return alpha;
        }

        private Stack<Checker> DoSequence(Move[] sequence)
        {
            var hits = new Stack<Checker>();
            foreach (var move in sequence)
            {
                if (move == null)
                    continue;
                var hit = Game.MakeMove(move);
                //if (hit != null)
                //    Debug.Assert(true);
                hits.Push(hit);
            }
            Game.SwitchPlayer();
            return hits;
        }

        private void UndoSequence(Move[] sequence, Stack<Checker> hits)
        {
            Game.SwitchPlayer();

            for (int i = sequence.Length - 1; i >= 0; i--)
                if (sequence[i] != null)
                    Game.UndoMove(sequence[i], hits.Pop());
        }

        private double EvaluatePoints(Player.Color myColor)
        {
            if (myColor == Player.Color.Black)
                return Game.BlackPlayer.PointsLeft - Game.WhitePlayer.PointsLeft;
            else
                return Game.WhitePlayer.PointsLeft - Game.BlackPlayer.PointsLeft;
        }

        private double EvaluateCheckers(Player.Color myColor)
        {
            double score = 0;
            var inBlock = false;
            var counter = 0;
            for (int i = 1; i < 25; i++)
            {
                var point = Game.Points[i];
                if (point.MyBlock(myColor))
                {
                    if (inBlock)
                        counter++;
                    else
                        counter = 1;
                    inBlock = true;
                }
                else
                {
                    if (inBlock)
                    {
                        score += Math.Pow(counter, 2);
                        counter = 0;
                    }
                    inBlock = false;
                    if (Configuration.HitableBad && point.Hitable(myColor))
                    {
                        score -= point.GetNumber(myColor) / 10;
                    }
                }
            }

            if (inBlock)
                score += Math.Pow(counter, 2);

            score += Game.GetHome(myColor).Checkers.Count * 10;
            return score;
        }

        //Get the average score for current player rolling all possible combinations
        private double PropabilityScore(Player.Color myColor)
        {
            var allDiceRoll = AllRolls();
            var scores = new List<double>();
            foreach (var roll in allDiceRoll)
            {
                Game.FakeRoll(roll.dice1, roll.dice2);
                var bestScore = double.MinValue;
                var seqs = GenerateMovesSequence();
                foreach (var s in seqs)
                {
                    var hits = DoSequence(s);
                    var score = EvaluatePoints(myColor) + EvaluateCheckers(myColor);
                    if (score > bestScore)
                        bestScore = score;
                    UndoSequence(s, hits);
                }
                int m = roll.dice1 == roll.dice2 ? 1 : 2; // dice roll with not same value on dices are twice as probable. 2 / 36, vs 1 / 36
                if (seqs.Any())
                    scores.Add(bestScore * m);
                // Get best score of each roll, and make an average.
                // some rolls are more probable, multiply them
                // some rolls will be blocked or partially blocked
            }
            if (!scores.Any())
                return -100000;
            return scores.Average();
        }

        private (int dice1, int dice2)[] _allRolls = null;
        private (int dice1, int dice2)[] AllRolls()
        {
            if (_allRolls != null)
                return _allRolls;
            var list = new List<(int, int)>();
            for (int d1 = 1; d1 < 7; d1++)
                for (int d2 = 1; d2 < 7; d2++)
                {
                    if (!list.Contains((d1, d2)) && !list.Contains((d2, d1)))
                        list.Add((d1, d2));
                }
            _allRolls = list.ToArray();
            return _allRolls;
        }

        public List<Move[]> GenerateMovesSequence()
        {
            var sequences = new List<Move[]>();
            var moves = new Move[Game.Roll.Count];
            sequences.Add(moves);
            GenerateMovesSequence(sequences, moves, 0);

            // Special case. Sometimes the first dice is blocked, but can be moved after next dice
            if (sequences.Count == 1 && sequences[0].All(move => move == null))
            {
                var temp = Game.Roll[0];
                Game.Roll[0] = Game.Roll[1];
                Game.Roll[1] = temp;
                GenerateMovesSequence(sequences, moves, 0);
            }

            // If there are move sequences with all moves not null, remove sequences that has some moves null.
            // (rule of backgammon that you have to use all dice if you can)
            if (sequences.Any(moves => moves.All(m => m != null)))
                sequences = sequences.Where(moves => moves.All(m => m != null)).Select(s => s).ToList();
            return sequences;
        }

        private void GenerateMovesSequence(List<Move[]> sequences, Move[] moves, int diceIndex)
        {
            var bar = Game.Points.Where(p => p.GetNumber(Game.CurrentPlayer) == 0);
            var barHasCheckers = bar.First().Checkers.Any(c => c.Color == Game.CurrentPlayer);
            var dice = Game.Roll[diceIndex];

            var points = barHasCheckers ? bar :
                Game.Points.Where(p => p.Checkers.Any(c => c.Color == Game.CurrentPlayer))
                .OrderBy(p => p.GetNumber(Game.CurrentPlayer));

            foreach (var fromPoint in points)
            {
                var fromPointNo = fromPoint.GetNumber(Game.CurrentPlayer);
                if (fromPointNo == 25)
                    continue;
                var toPoint = Game.Points.SingleOrDefault(p => p.GetNumber(Game.CurrentPlayer) == dice.Value + fromPointNo);
                if (toPoint != null && toPoint.IsOpen(Game.CurrentPlayer)
                    && !toPoint.IsHome(Game.CurrentPlayer)) // no creation of bearing off moves here. See next block.
                {
                    var move = new Move { Color = Game.CurrentPlayer, From = fromPoint, To = toPoint };
                    //copy and make a new list for first dice
                    if (moves[diceIndex] == null)
                        moves[diceIndex] = move;
                    else // a move is already generated for this dice in this sequence. branch off a new.
                    {
                        var newMoves = new Move[Game.Roll.Count];
                        Array.Copy(moves, newMoves, diceIndex);
                        newMoves[diceIndex] = move;
                        if (diceIndex < Game.Roll.Count - 1 || !sequences.ContainsEntryWithAll(newMoves))
                        {
                            moves = newMoves;
                            sequences.Add(moves);
                        }
                    }

                    if (diceIndex < Game.Roll.Count - 1) // Do the created move and recurse to next dice
                    {
                        var hit = Game.MakeMove(move);
                        GenerateMovesSequence(sequences, moves, diceIndex + 1);
                        Game.UndoMove(move, hit);
                    }
                }
                else if (Game.IsBearingOff(Game.CurrentPlayer))
                {
                    // The furthest away checker can be moved beyond home.
                    var minPoint = Game.Points.Where(p => p.Checkers.Any(c => c.Color == Game.CurrentPlayer)).OrderBy(p => p.GetNumber(Game.CurrentPlayer)).First().GetNumber(Game.CurrentPlayer);
                    var toPointNo = fromPointNo == minPoint ? Math.Min(25, fromPointNo + dice.Value) : fromPointNo + dice.Value;
                    toPoint = Game.Points.SingleOrDefault(p => p.GetNumber(Game.CurrentPlayer) == toPointNo);
                    if (toPoint != null && toPoint.IsOpen(Game.CurrentPlayer))
                    {
                        var move = new Move { Color = Game.CurrentPlayer, From = fromPoint, To = toPoint };
                        if (moves[diceIndex] == null)
                            moves[diceIndex] = move;
                        else
                        {
                            var newMoves = new Move[Game.Roll.Count];
                            Array.Copy(moves, newMoves, diceIndex);
                            newMoves[diceIndex] = move;
                            if (diceIndex < Game.Roll.Count - 1 || !sequences.ContainsEntryWithAll(newMoves))
                            {
                                moves = newMoves;
                                sequences.Add(moves);
                            }
                        }
                        if (diceIndex < Game.Roll.Count - 1)
                        {
                            var hit = Game.MakeMove(move);
                            GenerateMovesSequence(sequences, moves, diceIndex + 1);
                            Game.UndoMove(move, hit);
                        }
                    }
                }
            }
        }
    }
}

static class Extensions
{
    public static bool ContainsEntryWithAll(this List<Move[]> listOfList, Move[] match)
    {
        // searching for a list entry that contains all entries in match
        foreach (var list in listOfList)
        {
            var hasMove = true;
            foreach (var mv in match)
            {
                if (!list.Any(m => mv != null && m != null && m.From == mv.From && m.To == mv.To && m.Color == mv.Color))
                {
                    hasMove = false;
                    break;
                }
            }
            if (hasMove)
                return true;

        }
        return false;
    }

    public static string BuildString(this Move[] moves)
    {
        return string.Join(",", moves.Where(m => m != null).Select(m => m.ToString()).ToArray());
    }
}

