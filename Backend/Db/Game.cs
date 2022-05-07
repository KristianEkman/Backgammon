using Backend.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Db
{
    public class Game
    {
        public Guid Id { get; set; }
        public DateTime UtcStarted { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
        public PlayerColor? Winner { get; set; }
    }
}
