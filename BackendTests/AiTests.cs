using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests
{
    [TestClass]
    public class AiTests
    {
        [TestMethod]
        public void GetBestMove()
        {
            var game = Backend.Rules.Game.Create();
            var ai = new Ai.Engine(game);
            game.FakeRoll(2, 3);
            var moves = ai.GetBestMoves();
            foreach (var move in moves)
            {
                Console.WriteLine(move);
            }
        }
    }
}
