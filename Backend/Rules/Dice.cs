using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    public class Dice
    {

        private static Random Random = new Random();

        public static int RollOne()
        {
            return Random.Next(1, 7);
        }

        public static (int, int) Roll()
        {
            return (RollOne(), RollOne());
        }
    }
}
