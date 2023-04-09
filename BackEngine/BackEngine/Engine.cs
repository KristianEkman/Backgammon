using System;
using System.Diagnostics;

namespace BackEngine
{
	public class Engine
	{
		public Engine()
		{
	       SetStartPosition();
        }

        public const sbyte Black = -1;
        public const sbyte White = 1;


        public Board Board { get; private set; } = new Board();

        public void SetStartPosition()
        {
			for (int i = 0; i < 26; i++)
				Board.Spots[i] = 0;
			Board.Spots[1] = 2;
			Board.Spots[6] = -5;
			Board.Spots[8] = -3;
			Board.Spots[12] = 5;
			Board.Spots[13] = -5;
			Board.Spots[17] = 3;
			Board.Spots[19] = 5;
			Board.Spots[24] = -2;
        }

		/// <returns>True if oponent checker was hit.</returns>
        public bool DoMove(Move move)
        {
			var hit = false;
			var off = false;
			if (move.To == 25)
			{
				Debug.Assert(move.Side == Engine.White);
				Board.WhiteHome++;
				off = true;
			}
			else if(move.To == 0)
			{
                Debug.Assert(move.Side == Engine.Black);
                Board.BlackHome++;
                off = true;
            }
            else if (Board.Spots[move.To] == -move.Side)
			{
				// Debug.Assert(Math.Abs(Board.Spots[move.To]) > -2);
				if (move.Side == Engine.Black)
					Board.Spots[0]++;
				else
					Board.Spots[25]--; // Black hit checkers have negative value, even on the bar.
				Board.Spots[move.To] = 0;
				hit = true;
            }
			Board.Spots[move.From] -= move.Side;

			if (!off)
				Board.Spots[move.To] += move.Side;
			return hit;
        }

        public void UndoMove(Move move, bool hit)
        {
			var off = false;
            if (move.To == 25)
            {
                Debug.Assert(move.Side == Engine.White);
                Board.WhiteHome--;
                off = true;
            }
            else if (move.To == 0)
            {
                Debug.Assert(move.Side == Engine.Black);
                Board.BlackHome--;
                off = true;
            }
            else if (hit)
			{
				if (move.Side == Engine.Black)
					Board.Spots[0]--;
				else
                    Board.Spots[25]++;
                Board.Spots[move.To] = (sbyte)-move.Side;
            }
            Board.Spots[move.From] += move.Side;
			if (!off && !hit)
				Board.Spots[move.To] -= move.Side;
        }

        public void CreateMoves(Generation gen, sbyte side)
		{
			gen.HashSet.Clear();
			gen.GeneratedCount = 0;

			if (side == White)
			{
				CreateMovesWhite(gen, 0);
				if (gen.Dice.Length == 2) // dice are not the same
				{
                    var dice0 = gen.Dice[0];
					gen.Dice[0] = gen.Dice[1];
					gen.Dice[1] = dice0;
                    CreateMovesWhite(gen, 0);
                }
            } else {
                CreateMovesBlack(gen, 0);
                if (gen.Dice.Length == 2) // dice are not the same
                {
                    var dice0 = gen.Dice[0];
                    gen.Dice[0] = gen.Dice[1];
                    gen.Dice[1] = dice0;
                    CreateMovesBlack(gen, 0);
                }
            }
        }


        private void CreateMovesWhite(Generation gen, int currentDiceIdx)
        {
            var bearingOff = true;
            var firstCheckerIndex = 0;
            for (int i = 0; i < 25; i++)
            {
                if (Board.Spots[i] > 0)
                {
                    firstCheckerIndex = i;   
                    bearingOff = (i >= 19);
                    break;
                }
            }

            for (int i = firstCheckerIndex; i < 25; i++)
			{
				if (Board.Spots[i] < 1) // no white here
					continue;
				var to = i + gen.Dice[currentDiceIdx];
                if (to > 24)
                {
                    if (!bearingOff)
                        break;
                    if (to > 25 && i != firstCheckerIndex)
                        break;
                    to = 25;
                }
                else
                {
                    if (Board.Spots[to] < -1)
                        continue; // blocked

                    if (i > 0 && Board.Spots[0] > 0)
                        break;
                }
				Move move;
				move.From = (byte)i;
				move.To = (byte)to;
				move.Side = White;
				gen.MoveSets[gen.GeneratedCount][currentDiceIdx] = move;

				if (currentDiceIdx == gen.Dice.Length - 1) //last dice
				{
                    gen.GeneratedCount++;
                    for (int d = 0; d < currentDiceIdx; d++)
                    {
                        gen.MoveSets[gen.GeneratedCount][d] =
                            gen.MoveSets[gen.GeneratedCount - 1][d];
                    }

					if (!gen.KeepSet())
                        gen.GeneratedCount--;
                    continue;
				}
                // Recurse to next dice
                var hit = DoMove(move);
				CreateMovesWhite(gen, currentDiceIdx + 1);
				UndoMove(move, hit);
            }
        }

        private void CreateMovesBlack(Generation gen, int currentDiceIdx)
        {
            var bearingOff = true;
            var firstCheckerIndex = 25;
            for (int i = 25; i > 0; i--)
            {
                if (Board.Spots[i] < 0)
                {
                    bearingOff = (i <= 6);
                    firstCheckerIndex = i;
                    break;
                }
            }

            for (int i = firstCheckerIndex; i >= 1; i--)
            {
                if (Board.Spots[i] > -1) //no black here
                    continue;
                var to = i - gen.Dice[currentDiceIdx];
                if (to < 1)
                {
                    if (!bearingOff)
                        break;
                    if (to < 0 && i != firstCheckerIndex)
                        break;
                    to = 0;
                } else {
                    if (Board.Spots[to] > 1)
                        continue; // blocked

                    if (i < 25 && Board.Spots[25] < 0)
                        break;
                }

                Move move;
                move.From = (byte)i;
                move.To = (byte)to;
                move.Side = Black;
                gen.MoveSets[gen.GeneratedCount][currentDiceIdx] = move;

                if (currentDiceIdx == gen.Dice.Length - 1) //last dice
                {
                    gen.GeneratedCount++;
                    for (int d = 0; d < currentDiceIdx; d++)
                    {
                        gen.MoveSets[gen.GeneratedCount][d] =
                            gen.MoveSets[gen.GeneratedCount - 1][d];
                    }
                    if (!gen.KeepSet())
                        gen.GeneratedCount--;
                    continue;
                }
                // Recurse to next dice
                var hit = DoMove(move);
                CreateMovesBlack(gen, currentDiceIdx + 1);
                UndoMove(move, hit);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Board.Spots.Length; i++)
            {
                Board.Spots[i] = 0;
            }
            Board.BlackHome = 0;
            Board.WhiteHome = 0;
        }
    }
}
