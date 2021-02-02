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

        internal int GetNumber(Player.Color player)
        {
            return player == Player.Color.Black ? BlackNumber : WhiteNumber;
        }

        public override string ToString()
        {
            var color = Checkers.Any() ? Checkers.First().Color.ToString() : "";
            return $"{Checkers.Count} {color} WN = {WhiteNumber}, BN = {BlackNumber}, ";
        }

        public bool IsHome(Player.Color player)
        {
            return this.GetNumber(player) == 25;
        }
    }
}
