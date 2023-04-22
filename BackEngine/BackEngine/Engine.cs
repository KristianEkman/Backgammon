using System;
using System.Diagnostics;
using System.Drawing;

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

        public Move[] GetBestMoveSet(int dice1, int dice2, int side)
		{
			// this is probably the bottleneck with managed language
			// I think C is much faster to allocation abunch of memory.
			// Its just not possible to re
			var gen = new Generation(dice1, dice2);
			gen.SetDice(dice1, dice2);
			Board.CreateMoves(gen, side);
			var bestSet = new Move[0];
			double bestScore = side == Board.White ? int.MinValue : int.MaxValue;
			var setSize = 2;
			if (dice1 == dice2)
				setSize = 4;
			var undids = new Undid[setSize];
			for (int i = 0; i < gen.GeneratedCount; i++)
			{
				var set = gen.MoveSets[i];
				for (int j = 0; j < setSize; j++)
					undids[j] = Board.DoMove(set[j]);

				var score = AverageOponentScore(-side);
				if (side == Board.White)
				{
					if (score > bestScore)
					{
						bestScore = score;
						bestSet = set;
					}
				} else {
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestSet = set;
                    }
                }
				
				//Undo
				for (int j = setSize - 1; j >= 0; j--)
					Board.UndoMove(set[j], undids[j]);
			}

			return bestSet;
		}

		private double AverageOponentScore(int side)
		{
			var list = new List<int>();
			for (int d = 0; d < AllDice.Length; d++)
			{
				var dice1 = AllDice[d].D1;
				var dice2 = AllDice[d].D2;
                var gen = new Generation(dice1, dice2);
                gen.SetDice(dice1, dice2);
                Board.CreateMoves(gen, side);
                var setSize = 2;
                if (dice1 == dice2)
                    setSize = 4;
                var undids = new Undid[setSize];
                for (int i = 0; i < gen.GeneratedCount; i++)
                {
                    var set = gen.MoveSets[i];
                    for (int j = 0; j < setSize; j++)
                        undids[j] = Board.DoMove(set[j]);

                    var score = Board.GetScore();
                    list.Add(score);
                    //Undo
                    for (int j = setSize - 1; j >= 0; j--)
                        Board.UndoMove(set[j], undids[j]);
                }
            }

			return list.Average();
		}
	}
}
