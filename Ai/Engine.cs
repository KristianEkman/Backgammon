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
            EngineGame = game;
            Configuration = new Config();
        }


        private Game EngineGame { get; }

        public Config Configuration { get; set; }

        public Move[] GetBestMoves()
        {
            Move[] bestMoveSequence = null;
            var bestScore = double.MinValue;
            var allSequences = GenerateMovesSequence();
            //return allSequences[0].ToArray();
            //int depth = Game.Roll.Count == 2 ? 0 : 0;
            var myColor = EngineGame.CurrentPlayer;            
            for (int s = 0; s < allSequences.Count; s++)
            {
                var sequence = allSequences[s];
                var hits = DoSequence(sequence);
                var score = EvaluatePoints(myColor) + EvaluateCheckers(myColor);
                //if (bestScore - score < 2) //only eval costly propability score if it is interesting
                //if (Configuration.PropabilityScore)
                //    score -= PropabilityScore(oponent) * Configuration.PropabilityFactor; // the possibility for the other player to score good.
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
                return PropabilityScore(EngineGame.CurrentPlayer);

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
                var hit = EngineGame.MakeMove(move);                
                hits.Push(hit);
            }
            EngineGame.SwitchPlayer();
            return hits;
        }

        private void UndoSequence(Move[] sequence, Stack<Checker> hits)
        {
            EngineGame.SwitchPlayer();

            for (int i = sequence.Length - 1; i >= 0; i--)
                if (sequence[i] != null)
                    EngineGame.UndoMove(sequence[i], hits.Pop());
        }

        private double EvaluatePoints(Player.Color myColor)
        {
            if (myColor == Player.Color.White) // Higher score for white when few checkers and black has many checkers left
                return EngineGame.BlackPlayer.PointsLeft - EngineGame.WhitePlayer.PointsLeft;
            else
                return EngineGame.WhitePlayer.PointsLeft - EngineGame.BlackPlayer.PointsLeft;
        }

        private double EvaluateCheckers(Player.Color myColor)
        {
            double score = 0;
            var inBlock = false;
            var counter = 0;
            var cht = Configuration.HitableThreshold;
            var chf = (double)Configuration.HitableFactor;
            var cbf = Configuration.ConnectedBlocksFactor;
            var cbp = Configuration.BlockedPointScore;

            var other = myColor == Player.Color.Black ? Player.Color.White : Player.Color.Black;
            // Oponents checker closest to their bar. Relative to my point numbers.
            var opponentMax = EngineGame.Points.Where(p => p.Checkers.Any( c => c.Color == other)).Select( p => p.GetNumber(myColor)).Max();

            for (int i = 1; i < 25; i++)
            {
                var point = EngineGame.Points[i];
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
                    if (inBlock && point.GetNumber(myColor) < opponentMax) // If all opponents checkers has passed this block, its no use.
                    {
                        score += Math.Pow(counter * cbp, cbf);
                        counter = 0;
                    }
                    inBlock = false;
                    if (point.Hitable(myColor) && point.GetNumber(myColor) > cht)
                    {
                        score -= point.GetNumber(myColor) / chf;
                    }
                }
            }

            if (inBlock)
                score += Math.Pow(counter, 2);

            score += EngineGame.GetHome(myColor).Checkers.Count * 10;
            return score;
        }

        //Get the average score for current player rolling all possible combinations
        private double PropabilityScore(Player.Color myColor)
        {
            var allDiceRoll = AllRolls();
            var scores = new List<double>();
            foreach (var roll in allDiceRoll)
            {
                EngineGame.FakeRoll(roll.dice1, roll.dice2);
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
            var moves = new Move[EngineGame.Roll.Count];
            sequences.Add(moves);
            GenerateMovesSequence(sequences, moves, 0);

            // Special case. Sometimes the first dice is blocked, but can be moved after next dice
            if (sequences.Count == 1 && sequences[0].All(move => move == null))
            {
                var temp = EngineGame.Roll[0];
                EngineGame.Roll[0] = EngineGame.Roll[1];
                EngineGame.Roll[1] = temp;
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
            //var bar = Game.Points.Where(p => p.GetNumber(Game.CurrentPlayer) == 0);
            var current = EngineGame.CurrentPlayer;
            var bar = EngineGame.Bars[(int)current];
            var barHasCheckers = bar.Checkers.Any(c => c.Color == current);
            var dice = EngineGame.Roll[diceIndex];

            var points = barHasCheckers ? new[] { bar } :
                EngineGame.Points.Where(p => p.Checkers.Any(c => c.Color == current))
                //.OrderBy(p => p.GetNumber(current))
                .ToArray();

            // There seems to be a big advantage to evaluate points from lowest number.
            // If not reversing black will win 60 to 40 with same config.
            if (EngineGame.CurrentPlayer == Player.Color.White)
                Array.Reverse(points);

            foreach (var fromPoint in points)
            {
                var fromPointNo = fromPoint.GetNumber(EngineGame.CurrentPlayer);
                if (fromPointNo == 25)
                    continue;
                var toPoint = EngineGame.Points.SingleOrDefault(p => p.GetNumber(EngineGame.CurrentPlayer) == dice.Value + fromPointNo);
                if (toPoint != null && toPoint.IsOpen(EngineGame.CurrentPlayer)
                    && !toPoint.IsHome(EngineGame.CurrentPlayer)) // no creation of bearing off moves here. See next block.
                {
                    var move = new Move { Color = EngineGame.CurrentPlayer, From = fromPoint, To = toPoint };
                    //copy and make a new list for first dice
                    if (moves[diceIndex] == null)
                        moves[diceIndex] = move;
                    else // a move is already generated for this dice in this sequence. branch off a new.
                    {
                        var newMoves = new Move[EngineGame.Roll.Count];
                        Array.Copy(moves, newMoves, diceIndex);
                        newMoves[diceIndex] = move;
                        if (diceIndex < EngineGame.Roll.Count - 1 || !sequences.ContainsEntryWithAll(newMoves))
                        {
                            moves = newMoves;
                            sequences.Add(moves);
                        }
                    }

                    if (diceIndex < EngineGame.Roll.Count - 1) // Do the created move and recurse to next dice
                    {
                        var hit = EngineGame.MakeMove(move);
                        GenerateMovesSequence(sequences, moves, diceIndex + 1);
                        EngineGame.UndoMove(move, hit);                        
                    }
                }
                else if (EngineGame.IsBearingOff(EngineGame.CurrentPlayer))
                {
                    // The furthest away checker can be moved beyond home.
                    var minPoint = EngineGame.Points.Where(p => p.Checkers.Any(c => c.Color == EngineGame.CurrentPlayer)).OrderBy(p => p.GetNumber(EngineGame.CurrentPlayer)).First().GetNumber(EngineGame.CurrentPlayer);
                    var toPointNo = fromPointNo == minPoint ? Math.Min(25, fromPointNo + dice.Value) : fromPointNo + dice.Value;
                    toPoint = EngineGame.Points.SingleOrDefault(p => p.GetNumber(EngineGame.CurrentPlayer) == toPointNo);
                    if (toPoint != null && toPoint.IsOpen(EngineGame.CurrentPlayer))
                    {
                        var move = new Move { Color = EngineGame.CurrentPlayer, From = fromPoint, To = toPoint };
                        if (moves[diceIndex] == null)
                            moves[diceIndex] = move;
                        else
                        {
                            var newMoves = new Move[EngineGame.Roll.Count];
                            Array.Copy(moves, newMoves, diceIndex);
                            newMoves[diceIndex] = move;
                            if (diceIndex < EngineGame.Roll.Count - 1 || !sequences.ContainsEntryWithAll(newMoves))
                            {
                                moves = newMoves;
                                sequences.Add(moves);
                            }
                        }
                        if (diceIndex < EngineGame.Roll.Count - 1)
                        {
                            var hit = EngineGame.MakeMove(move);
                            GenerateMovesSequence(sequences, moves, diceIndex + 1);
                            EngineGame.UndoMove(move, hit);                            
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

