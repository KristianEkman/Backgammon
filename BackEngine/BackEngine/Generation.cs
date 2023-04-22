using System;
using System.Diagnostics;

namespace BackEngine
{
	public class Generation
	{

		public Generation(int dice1, int dice2)
		{
			SetDice(dice1, dice2);

			// This class is supposed to be reused between generations
			// Thats why extra memory is used.
            MoveSets = new Move[1000][];
            for (int i = 0; i < MoveSets.Length; i++)
                MoveSets[i] = new Move[4];
        }

		public void SetDice(int dice1, int dice2)
		{
			// todo: always keep them and have a DiceLength
            if (dice1 == dice2)
            {
                Dice = new int[] { dice1, dice1, dice2, dice2 };
            }
            else
            {
                Dice = new int[] { dice1, dice2 };
            }
        }

		private static Randoms Randoms = new();

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

#if DEBUG
        public HashSet<string> DebugHashSet = new();
        internal bool DebugKeepSet(string board)
        {
            if (DebugHashSet.Contains(board))
                return false;
            DebugHashSet.Add(board);
            return true;
        }
#endif

        public HashSet<uint> HashSet = new();

        internal bool KeepSet(uint hash)
        {				
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
            PlaceCount = new uint[26, 31];
			for (int x = 0; x < 26; x++)
			{
				for (int y = 0; y < 31; y++)
				{
                    PlaceCount[x, y] = (uint)rnd.Next();
                }
			}
		}

		//Random big numbers for every combination of checkourcount, place.
		//Relying on them to be unique.
		//Used for Zobrits hashing a board position.
		//Xor when checker is put on or off.
		public uint[,] PlaceCount
		{
			get;
			private set;
		}

        public uint Start { get; private set; }
    }
}

