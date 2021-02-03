using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    public class Dice
    {
        public int Value { get; set; }
        public bool Used { get; set; }

        private static Random Random = new Random();

        public static int RollOne()
        {
            return Random.Next(1, 7);
        }

        public static Dice[] Roll()
        {
            var val1 = RollOne();
            var val2 = RollOne();
            return GetDices(val1, val2);
        }

        internal static Dice[] GetDices(int val1, int val2)
        {
            if (val1 == val2)
                return new[]
                {
                    new Dice{Value = val1},
                    new Dice{Value = val1},
                    new Dice{Value = val1},
                    new Dice{Value = val1},
                };

            return new[]
                {
                    new Dice{Value = val1},
                    new Dice{Value = val2},
                };
        }

        public override string ToString()
        {
            return Value + (Used ? " Used" : " Not used");
        }
    }
}
