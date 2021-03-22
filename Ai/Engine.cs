using Backend.Rules;
using System;
using System.Collections.Generic;

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
            var moves = Game.GenerateMoves();

            foreach (var move in moves)
            {
                move.Score = GetScore(move);
                var hit = Game.MakeMove(move);
                //evaluate
                Game.UndoMove(move, hit);
            }

            return null;
        }

        private int GetScore(Move move)
        {
            if (move.NextMoves.Count == 0)
                return 0; // Evaluate
            var score = 0;
            foreach (var m in move.NextMoves)
            {
                var s = GetScore(m);
                if (s > score)
                    score = s;
            }
            return score;
        }

        // antal points left.
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
                    list.Add((d1, d2));
            _allRolls = list.ToArray();
            return _allRolls;
        }
    }
}
