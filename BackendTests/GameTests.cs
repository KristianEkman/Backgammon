using Backend.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace BackendTests
{
    [TestClass]
    public class GameTests
    {
        private Game game;

        [TestInitialize]
        public void InitTests()
        {
            game = Game.Create();
        }

        [TestMethod]
        public void TestNewGame()
        {
            Assert.AreEqual(25, game.Points.Count);
            Assert.AreEqual(15, game.WhitePlayer.Checkers.Count);
            Assert.AreEqual(15, game.BlackPlayer.Checkers.Count);

            game.BlackPlayer.Checkers.TrueForAll(c => c.Color == Player.Color.Black);
            game.WhitePlayer.Checkers.TrueForAll(c => c.Color == Player.Color.White);

        }

        [TestMethod]
        public void TestRoll()
        {
            game.RollDice();
            Assert.IsNotNull(game.Roll);
        }

        [TestMethod]
        public void TestValidMove()
        {
            game.FakeRoll(1, 2);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            
            Assert.AreEqual(6, moves.Count);
        }
    }
}
