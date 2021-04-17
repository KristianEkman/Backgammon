using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    /// <summary>
    /// Represents one of 24 a triangles where a checker can be placed.
    /// 1 to 24.
    /// The bar is 0. Beared off is 25.
    /// </summary>
    public class Point
    {
        public List<Checker> Checkers { get; set; } = new List<Checker>();
        public int WhiteNumber { get; set; }
        public int BlackNumber { get; set; }

        public bool IsOpen(Player.Color myColor)
        {
            //Opponent has less than two checkers on the point.
            //My own home is always open.
            return Checkers.Count(c => c.Color != myColor) < 2 || GetNumber(myColor) == 25;
        }

        public bool MyBlock(Player.Color myColor)
        {
            // Do I have a block? Home doesnt count.
            return Checkers.Count(c => c.Color == myColor) >= 2;
        }

        public int GetNumber(Player.Color player)
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

        public bool Bloat(Player.Color myColor)
        {
            return Checkers.Count(c => c.Color == myColor)== 1;
        }
    }
}
