using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class Player
    {
        public Guid Id { get; set; }
        public Color Color { get; set; }

        public User User { get; set; }

        public Game Game { get; set; }

    }

    public enum Color
    {
        Black,
        White
    }
}
