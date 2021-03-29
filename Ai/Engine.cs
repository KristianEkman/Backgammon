using Backend.Rules;
using System;
using System.Collections.Generic;
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
            var bestScore = -10000;
            var allSequences = GetAllMoveSequence();
            foreach (var sequence in allSequences)
            {
                var score = GetScoreOfSequence(sequence);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoveSequence = sequence;
                }
            }

            return bestMoveSequence.ToArray();

        }

        private int GetScoreOfSequence(List<Move> sequence)
        {
            var hits = new Stack<Checker>();
            foreach (var move in sequence)
            {
                var hit = Game.MakeMove(move);
                hits.Push(hit);
            }

            var score = Evaluate();

            for (int i = sequence.Count - 1; i >= 0; i--)
            {
                Game.UndoMove(sequence[i], hits.Pop());
            }
            return 0;
        }

        private List<List<Move>> GetAllMoveSequence()
        {
            // make an 2d-array of the move tree structure.
            var validMoves = Game.GenerateMoves();
            var listofList = new List<List<Move>>();
            var first1 = true;
            var first2 = true;
            var first3 = true;
            var first4 = true;
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
                first2 = true;

                foreach (var move2 in move1.NextMoves)
                {
                    if (first2)
                        listofList[idx].Add(move2);
                    else
                    {
                        var take = listofList[idx].Count - 1;
                        listofList.Add(new List<Move>(listofList[idx].Take(take).ToArray()));
                        idx++;
                        listofList[idx].Add(move2);
                    }
                    first2 = false;
                    first3 = true;

                    foreach (var move3 in move2.NextMoves)
                    {
                        if (first3)
                            listofList[idx].Add(move3);
                        else
                        {
                            var take = listofList[idx].Count - 1;
                            listofList.Add(new List<Move>(listofList[idx].Take(take).ToArray()));
                            idx++;
                            listofList[idx].Add(move3);
                        }
                        first3 = false;
                        first4 = true;

                        foreach (var move4 in move3.NextMoves) // this is maximum number of moves in one roll
                        {
                            if (first4)
                                listofList[idx].Add(move4);
                            else
                            {
                                var take = listofList[idx].Count - 1;
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
            var diff = 0;
            if (Game.CurrentPlayer == Player.Color.Black)
                diff = Game.BlackPlayer.PointsLeft - Game.WhitePlayer.PointsLeft;
             else
                diff = Game.WhitePlayer.PointsLeft - Game.BlackPlayer.PointsLeft;

            // give points for consecutive  blocked points

            // propability of being hit next time.
            //var propScore = PropabilityScore();

            return diff;
        }

        private int PropabilityScore()
        {
            var allDiceRoll = AllRolls();
            foreach (var roll in allDiceRoll)
            {                
                Game.FakeRoll(roll.dice1, roll.dice2);
                // calc average score of all hits
                var moves = Game.ValidMoves;
            }
            return 0;
        }

        // antal points left - opponent points .
        // sannolikheten för att bli träffad
        //  Räkna Evaluera alla  möjliga Rolls
        //  Om sannolikheten är låg att man blir träffad
        //  men hög att man kan blockera motståndaren så är det bra.

        //  medel points left.


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
}
