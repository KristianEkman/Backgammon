using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    public class Player
    {
        public Color PlayerColor { get; set; }

        public enum Color
        {
            Black,
            White,
            Neither
        }

        public override string ToString()
        {
            return PlayerColor + " player";
        }
    }
}
