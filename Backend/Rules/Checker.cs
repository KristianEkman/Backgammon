using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    public class Checker
    {
        public Player.Color Color { get; set; }

        public override string ToString()
        {
            return Color.ToString();
        }
    }
}
