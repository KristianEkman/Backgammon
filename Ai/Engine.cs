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
        }

        private Game Game { get; }

        public Move[] GetBestMoves()
        {
            List<Move> bestMoveSequence = null;
            var bestScore = double.MinValue;
            var allSequences = GetAllMoveSequence();
            //return allSequences[0].ToArray();

            foreach (var sequence in allSequences)
            {                                
                var score = AlpaBeta(int.MinValue, int.MaxValue, 0);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoveSequence = sequence;
                }
            }
            return bestMoveSequence.ToArray();
        }

        private double AlpaBeta(double alpha, double beta, int depth)
        {
            if (depth == 0)
                return PropabilityScore();

            var bestScore = double.MinValue;
            var seqs = GetAllMoveSequence();

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

        private Stack<Checker> DoSequence(List<Move> sequence)
        {
            var hits = new Stack<Checker>();
            foreach (var move in sequence)
            {
                var hit = Game.MakeMove(move);
                if (hit != null)
                    Debug.Assert(true);
                hits.Push(hit);
            }
            Game.SwitchPlayer();
            return hits;
        }

        private void UndoSequence(List<Move> sequence, Stack<Checker> hits)
        {
            Game.SwitchPlayer();

            for (int i = sequence.Count - 1; i >= 0; i--)
            {
                Game.UndoMove(sequence[i], hits.Pop());
            }
        }

        private List<List<Move>> GetAllMoveSequence()
        {
            // make an 2d-array of the move tree structure.

            // todo: unique movesequences, undependent on order
            var validMoves = Game.GenerateMoves();
            var listofList = new List<List<Move>>();
            var first1 = true;
            listofList.Add(new List<Move>());
            int idx = 0;
            foreach (var move1 in validMoves)
            {
                if (first1)
                    listofList[idx].Add(move1);
                else
                {
                    listofList.Add(new List<Move>());
                    idx++;
                    listofList[idx].Add(move1);
                }
                first1 = false;
                var first2 = true;

                foreach (var move2 in move1.NextMoves)
                {
                    if (first2)
                        listofList[idx].Add(move2);
                    else
                    {
                        var copy = listofList[idx].Take(1).ToList();
                        copy.Add(move2);
                        if (listofList.ContainsEntryWithAll(copy))
                            continue;

                        listofList.Add(new List<Move>(copy.ToArray()));
                        idx++;
                    }
                    first2 = false;
                    var first3 = true;

                    foreach (var move3 in move2.NextMoves)
                    {
                        if (first3)
                            listofList[idx].Add(move3);
                        else
                        {
                            var take = 2;
                            listofList.Add(new List<Move>(listofList[idx].Take(take).ToArray()));
                            idx++;
                            listofList[idx].Add(move3);
                        }
                        first3 = false;
                        var first4 = true;

                        foreach (var move4 in move3.NextMoves) // this is maximum number of moves in one roll
                        {
                            if (first4)
                                listofList[idx].Add(move4);
                            else
                            {
                                var take = 3;
                                listofList.Add(new List<Move>(listofList[idx].Take(take).ToArray()));
                                idx++;
                                listofList[idx].Add(move4);
                            }
                            first4 = false;
                        }
                    }
                }
            }
            return listofList;
        }

        private int Evaluate()
        {
            var score = 0;
            var myColor = Game.CurrentPlayer;
            if (myColor == Player.Color.Black)
                score = Game.BlackPlayer.PointsLeft - Game.WhitePlayer.PointsLeft;
             else
                score = Game.WhitePlayer.PointsLeft - Game.BlackPlayer.PointsLeft;

            var inBlock = false;
            var counter = 0;
            for (int i = 0; i < Game.Points.Count; i++)
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
                        score += (int)Math.Pow(counter, 2);
                        counter = 0;

                    }
                    inBlock = false;
                }
            }

            if (inBlock)
                score += (int)Math.Pow(counter, 2);

            return score;
        }

        private double PropabilityScore()
        {
            var allDiceRoll = AllRolls();
            var scores = new List<int>();
            foreach (var roll in allDiceRoll)
            {                           
                Game.FakeRoll(roll.dice1, roll.dice2);
                var bestScore = int.MinValue;
                var seqs = GetAllMoveSequence();
                foreach (var s in seqs)
                {
                    var hits = DoSequence(s);
                    var score = Evaluate();
                    if (score > bestScore)
                        bestScore = score;
                    UndoSequence(s, hits);
                }
                int m = roll.dice1 == roll.dice2 ? 1 : 2; // dice roll with not same value on dices are twice as probable. 2 / 36, vs 1 / 36
                scores.Add(bestScore * m);
                // Get best score of each roll, and make an average.
                // some rolls are more probable, multiply them
                // some rolls will be blocked or partially blocked
            }
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
    }

    static class Extension
    {
        public static bool ContainsEntryWithAll(this List<List<Move>> listOfList, List<Move> match)
        {
            // searching for a list entry that contains all entries in match
            foreach (var list in listOfList)
            {
                var hasMove = true;
                foreach (var mv in match)
                {
                    if (!list.Any(m => m.From == mv.From && m.To == mv.To && m.Color == mv.Color))
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
    }
}
