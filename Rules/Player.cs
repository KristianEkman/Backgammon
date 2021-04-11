using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    public class Player
    {
        public string Name { get; set; }
        public Color PlayerColor { get; set; }
        public int PointsLeft { get; set; }

        /// <summary>
        /// Do not map this to the dto. Opponnents id should never be revealed to anyone else.
        /// </summary>
        public Guid Id { get; set; }

        public bool FirstMoveMade { get; set; }

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

        public bool IsGuest()
        {
            return Id == Guid.Empty;
        }

    }
}
