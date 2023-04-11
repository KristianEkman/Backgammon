using System;
using System.Diagnostics;

namespace BackEngine
{
	public class Generation
	{
		public Generation(int dice1, int dice2)
		{
			if (dice1 == dice2)
			{
				Dice = new int[] { dice1, dice1, dice2, dice2 };
			}
			else
			{
				Dice = new int[] { dice1, dice2 };
            }

			// This class is supposed to be reused between generations
			// Thats why extra memory is used.
            MoveSets = new Move[1000][];
            for (int i = 0; i < MoveSets.Length; i++)
                MoveSets[i] = new Move[4];
        }

		private static Randoms Randoms = new Randoms();

		public int[] Dice { get; set; }

		// Keeps track of number of moves generated for current dice. Faster than list.
		public int GeneratedCount { get; set; }

        public bool HasFullSets { get; set; }
        public bool HasPartialSets { get; set; }

        /// <summary>
		/// [moveindex][diceIdx]
		/// Each dice generate a list of moves
		/// </summary>
        public Move[][] MoveSets { get; set; }

		public HashSet<uint> HashSet = new();

        internal bool KeepSet()
        {
			// 1-3, 3-6 or 1-4, 4-6, not same thing
			// but 1-3, 2-5 or 2-5, 1-3, is always same set.
			var hash = Randoms.Start;
			for (int i = 0; i < Dice.Length; i++)
			{
				var move = MoveSets[GeneratedCount - 1][i];
                hash ^= Randoms.FromTo[move.From, move.To];
			}
			if (HashSet.Contains(hash))
				return false;
			HashSet.Add(hash);
			return true;
        }

        public void PrintMoves()
        {
            for (int i = 0; i < GeneratedCount; i++)
            {
                for (int j = 0; j < Dice.Length; j++)
                {
                    Console.Write(MoveSets[i][j]);
                    Console.Write("\t");
                }
                Console.WriteLine("");
            }
        }
    }

	public class Randoms
	{
		public Randoms()
		{
			var rnd = new Random(1000);
			Start = (uint)rnd.Next();
            FromTo = new uint[26, 26];
			for (int x = 0; x < 26; x++)
			{
				for (int y = 0; y < 26; y++)
				{
					FromTo[x, y] = (uint)rnd.Next();
                }
			}
		}

		//Random big numbers for every combination of from, to.
		//Relying on them to be unique.
		//Used for Zobrits hashing a set of moves.
		public uint[,] FromTo
		{
			get;
			private set;
		}

        public uint Start { get; private set; }
    }
}

