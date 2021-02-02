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
        public void TestRoll()
        {
            game.RollDice();
            Assert.IsNotNull(game.Roll);
        }

        [TestMethod]
        public void TestMoveGeneration()
        {
            game.FakeRoll(1, 2);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            
            Assert.AreEqual(7, moves.Count);
        }

        [TestMethod]
        public void TestMoveGenerationDoubles()
        {
            game.FakeRoll(2, 2);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            Assert.AreEqual(4, moves.Count);
        }

        [TestMethod]
        public void TestCheckerOnTheBarBlocked()
        {
            game.AddCheckers(2, Player.Color.Black, 0);
            game.FakeRoll(6, 6);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            Assert.AreEqual(0, moves.Count);
        }

        [TestMethod]
        public void TestCheckerOnTheBar()
        {
            game.AddCheckers(2, Player.Color.Black, 0);
            game.FakeRoll(5, 5);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            Assert.AreEqual(1, moves.Count);
        }

        [TestMethod]
        public void TestBearingOff()
        {
            game.ClearCheckers();
            game.AddCheckers(2, Player.Color.Black, 20);
            game.AddCheckers(2, Player.Color.Black, 21);
            game.AddCheckers(2, Player.Color.Black, 22);
            game.AddCheckers(2, Player.Color.Black, 23);
            game.AddCheckers(2, Player.Color.Black, 24);

            game.AddCheckers(2, Player.Color.White, 10);

            Assert.IsTrue(game.IsBearingOff(Player.Color.Black));
            Assert.IsFalse(game.IsBearingOff(Player.Color.White));

        }

        [TestMethod]
        public void TestNotBearingOff()
        {
            game.ClearCheckers();
            game.AddCheckers(2, Player.Color.Black, 19);
            game.AddCheckers(2, Player.Color.Black, 10);
            Assert.IsFalse(game.IsBearingOff(Player.Color.Black));

            game.FakeRoll(6, 6);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            Assert.AreEqual(1, moves.Count);
        }

        [TestMethod]
        public void TestBearingOffOvershot()
        {
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 21); //needs 4
            game.AddCheckers(1, Player.Color.Black, 22); //needs 3
            Assert.IsTrue(game.IsBearingOff(Player.Color.Black));

            game.FakeRoll(6, 5);
            List<Move> moves = new List<Move>();
            game.GenerateMoves(moves);
            Assert.AreEqual(1, moves.Count);
        }
    }
}
