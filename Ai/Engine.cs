using Backend.Rules;
using System;

namespace Ai
{
    public class Engine
    {
        public Engine(Game game)
        {
            Game = game;
        }

        private Game Game { get; }

        public Move[] GetBestMoves()
        {
            return null;
        }
    }
}
