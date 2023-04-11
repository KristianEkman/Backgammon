using System;
using System.Diagnostics;

namespace BackEngine
{
	public class Engine
	{
        public Board Board { get; private set; } = new Board();
		private readonly (int D1, int D2)[] AllDice = new (int D1, int D2)[21];

        public Engine() 
		{
			SetDiceCombinations();
		}

        private void SetDiceCombinations()
        {
            int i = 0;
            for (int a = 1; a < 7; a++)
                for (int b = a; b < 7; b++)
                    AllDice[i++] = (a, b);                    
        }

        public Move[] SearchBestMoveSet(int dice1, int dice2, int depth = 1)
		{
			// Iterate Depth Alpha beta search best move, w pruning.
			return new Move[0];
		}

		private double AverageScore()
		{
			//Average score for all oponent dice outcomes.

			//pips left
			//bloats, threatend by no. -
			//blocks, +
			//spots > count 3
			return 0;
		}
	}
}
