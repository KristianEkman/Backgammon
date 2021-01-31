using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    /// <summary>
    /// 1 to 24.
    /// ´The bar is 0. Beared off is 25.
    /// </summary>
    public class Point
    {
        public List<Checker> Checkers { get; set; } = new List<Checker>();
        public int WhiteNumber { get; set; }
        public int BlackNumber { get; set; }

        public bool IsOpen(Player.Color myColor)
        {
            //Oponent has less than two chechers on the point.
            return Checkers.Count(c => c.Color != myColor) < 2;
        }
    }
}
