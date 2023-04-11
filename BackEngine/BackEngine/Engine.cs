using System;
using System.Diagnostics;

namespace BackEngine
{
	public class Engine
	{
        public Board Board { get; private set; } = new Board();
		private readonly (byte D1, byte D2)[] AllDice = new (byte D1, byte D2)[21];

        public Engine() 
		{
			SetDiceCombinations();
		}

        void SetDiceCombinations()
        {
            int i = 0;
            for (int a = 1; a < 7; a++)
                for (int b = a; b < 7; b++)
                    AllDice[i++] = ((byte)a, (byte)b);                    
        }

        public Move[] SearchBestMoveSet(byte dice1, byte dice2, int depth = 1)
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
