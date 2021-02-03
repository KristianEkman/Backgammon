using Backend.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
            game.PlayState = Game.State.Playing;
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
            var moves = game.GenerateMoves();
            Assert.AreEqual(7, moves.Count);
        }

        [TestMethod]
        public void TestMoveGenerationDoubles()
        {
            game.FakeRoll(2, 2);
            var moves = game.GenerateMoves();
            Assert.AreEqual(4, moves.Count);
        }

        [TestMethod]
        public void TestCheckerOnTheBarBlocked()
        {
            game.AddCheckers(2, Player.Color.Black, 0);
            game.FakeRoll(6, 6);
            var moves = game.GenerateMoves();
            Assert.AreEqual(0, moves.Count);
        }

        [TestMethod]
        public void TestCheckerOnTheBar()
        {
            game.AddCheckers(2, Player.Color.Black, 0);
            game.FakeRoll(5, 5);
            var moves = game.GenerateMoves();
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
            var moves = game.GenerateMoves();
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
            var moves = game.GenerateMoves();
            Assert.AreEqual(1, moves.Count);
        }

        [TestMethod]
        public void TestHitMove()
        {
            // Adding a checker which will be hit.
            game.AddCheckers(1, Player.Color.White, 23);
            game.FakeRoll(1, 2);
            var moves = game.GenerateMoves();
            var move = moves.Single(m => m.From.BlackNumber == 1 && m.To.BlackNumber == 2);
            game.MakeMove(move);

            //one black each on point 1 and 2
            Assert.AreEqual(1, game.Points.Single(p => p.BlackNumber == 1).Checkers.Count);
            Assert.AreEqual(Player.Color.Black, game.Points.Single(p => p.BlackNumber == 1).Checkers.Single().Color);

            Assert.AreEqual(1, game.Points.Single(p => p.BlackNumber == 2).Checkers.Count);
            Assert.AreEqual(Player.Color.Black, game.Points.Single(p => p.BlackNumber == 2).Checkers.Single().Color);

            //one white on the bar
            Assert.AreEqual(1, game.Points.Single(p => p.WhiteNumber == 0).Checkers.Count);
            Assert.AreEqual(Player.Color.White, game.Points.Single(p => p.WhiteNumber == 0).Checkers.Single().Color);

            //Surprised this testmethod worked first time. Took about ten minutes to write.
        }

        [TestMethod]
        public void PlayBothDiceIfPossible()
        {
            game.ClearCheckers();
            game.AddCheckers(2, Player.Color.White, 1);
            game.AddCheckers(5, Player.Color.White, 12);
            game.AddCheckers(3, Player.Color.White, 18);
            game.AddCheckers(5, Player.Color.White, 19);
            
            game.AddCheckers(2, Player.Color.Black, 2);
            game.AddCheckers(2, Player.Color.Black, 3);
            game.AddCheckers(2, Player.Color.Black, 9);
            game.AddCheckers(3, Player.Color.Black, 19);
            game.AddCheckers(2, Player.Color.Black, 20);
            game.AddCheckers(2, Player.Color.Black, 22);
            game.AddCheckers(2, Player.Color.Black, 23);

            game.CurrentPlayer = Player.Color.White;
            game.FakeRoll(6, 4);

            var moves = game.GenerateMoves();
            Assert.IsFalse(moves.Any(m => m.From.WhiteNumber == 12 && m.To.WhiteNumber == 18));
        }

        
        [TestMethod]
        public void OnlyCanBePlayed()
        {
            game.ClearCheckers();
            var w = Player.Color.White;
            var b = Player.Color.Black;
            game.AddCheckers(1, w, 1);
            game.AddCheckers(4, w, 10);
            game.AddCheckers(5, w, 12);
            game.AddCheckers(6, w, 19);

            game.AddCheckers(2, b, 2);
            game.AddCheckers(2, b, 7);
            game.AddCheckers(2, b, 9);
            game.AddCheckers(2, b, 11);
            game.AddCheckers(2, b, 14);
            game.AddCheckers(2, b, 19);
            game.AddCheckers(1, b, 20);
            game.AddCheckers(2, b, 21);

            game.CurrentPlayer = w;
            game.FakeRoll(4, 6);
            var moves = game.GenerateMoves();
            Assert.IsTrue(moves.All(m => !m.NextMoves.Any()));
            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(6, moves.Single().To.WhiteNumber - moves.Single().From.WhiteNumber);
        }
    }
}
