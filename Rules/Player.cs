using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Rules
{
    public class Player
    {
        public const string AiUser = "ECC9A1FC-3E5C-45E6-BCE3-7C24DFE82C98";
        public string Name { get; set; }
        public Color PlayerColor { get; set; }
        public int PointsLeft { get; set; }
        public string Photo { get; set; }
        public int Gold { get; set; }
        public int Elo { get; set; }

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

        public bool IsAi()
        {
            return Id.ToString().Equals(Player.AiUser, StringComparison.OrdinalIgnoreCase);
        }

        internal Player Clone()
        {
            return new Player
            {
                FirstMoveMade = FirstMoveMade,
                Id = Id,
                Name = Name,
                PlayerColor = PlayerColor,
                PointsLeft = PointsLeft,
                Photo = Photo,
                Gold = Gold,
                Elo = Elo
            };
        }
    }
}
