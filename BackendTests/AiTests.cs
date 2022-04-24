using Ai;
using Backend.Rules;
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
        public void GetBestMoveBlack1()
        {
            var game = Game.Create(false);
            var ai = new Ai.Engine(game);
            game.ClearCheckers();

            game.AddCheckers(1, Player.Color.Black, 1);
            //game.AddCheckers(1, Player.Color.White, 1);

            game.AddCheckers(1, Player.Color.Black, 2);
            //game.AddCheckers(1, Player.Color.White, 2);

            game.AddCheckers(3, Player.Color.Black, 17);
            game.AddCheckers(3, Player.Color.White, 17);

            game.AddCheckers(1, Player.Color.White, 19); // 6 for black, the target

            game.FakeRoll(5, 4);
            var moves = ai.GetBestMoves();
            Assert.IsTrue(moves.SingleOrDefault(m => m.ToString() == "Black 1 -> 6") != null);
            Assert.IsTrue(moves.SingleOrDefault(m => m.ToString() == "Black 2 -> 6") != null);
        }

        [TestMethod]
        public void GetBestMoveWhite1()
        {
            var game = Game.Create(false);
            var ai = new Ai.Engine(game);
            game.ClearCheckers();

            game.AddCheckers(1, Player.Color.Black, 1);
            game.AddCheckers(1, Player.Color.White, 1);

            game.AddCheckers(1, Player.Color.Black, 2);
            game.AddCheckers(1, Player.Color.White, 2);

            game.AddCheckers(3, Player.Color.Black, 17);
            game.AddCheckers(3, Player.Color.White, 17);

            game.AddCheckers(1, Player.Color.Black, 19);

            game.FakeRoll(4, 5);
            game.SwitchPlayer();
            var moves = ai.GetBestMoves();

            Assert.IsTrue(moves.Any(m => m.ToString() == "White 1 -> 6"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "White 2 -> 6"));
        }

        [TestMethod]
        public void DoubleDice()
        {
            var game = Game.Create(false);
            var ai = new Ai.Engine(game);
            game.FakeRoll(2, 2);
            var moves = ai.GetBestMoves();

        }

        [TestMethod]
        public void FirstDiceBlocked()
        {
            var game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.White, 0);
            game.AddCheckers(1, Player.Color.White, 1);
            game.AddCheckers(2, Player.Color.White, 3);
            game.AddCheckers(2, Player.Color.White, 16);
            game.AddCheckers(2, Player.Color.White, 18);
            game.AddCheckers(2, Player.Color.White, 19);
            game.AddCheckers(2, Player.Color.White, 20);
            game.AddCheckers(2, Player.Color.White, 21);

            game.AddCheckers(1, Player.Color.Black, 3);
            game.AddCheckers(1, Player.Color.Black, 15);
            game.AddCheckers(2, Player.Color.Black, 16);
            game.AddCheckers(3, Player.Color.Black, 17);
            game.AddCheckers(3, Player.Color.Black, 19);
            game.AddCheckers(3, Player.Color.Black, 20);
            game.AddCheckers(2, Player.Color.Black, 21);

            game.CurrentPlayer = Player.Color.White;

            game.FakeRoll(5, 1);
            var ai = new Ai.Engine(game);
            var moves = ai.GetBestMoves();
            Assert.IsTrue(moves.Any(m => m != null));
        }

        [TestMethod]
        public void BearOffEffectivelyBlack()
        {
            var game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 19);
            game.AddCheckers(1, Player.Color.White, 19);
            game.AddCheckers(3, Player.Color.Black, 20);
            game.AddCheckers(3, Player.Color.White, 20);
            game.AddCheckers(3, Player.Color.Black, 21);
            game.AddCheckers(3, Player.Color.White, 21);
            game.AddCheckers(3, Player.Color.Black, 22);
            game.AddCheckers(3, Player.Color.White, 22);
            game.AddCheckers(3, Player.Color.Black, 23);
            game.AddCheckers(3, Player.Color.White, 23);
            game.AddCheckers(2, Player.Color.Black, 24);
            game.AddCheckers(2, Player.Color.White, 24);

            game.FakeRoll(1, 4);

            Assert.AreEqual(Player.Color.Black, game.CurrentPlayer);
            var ai = new Ai.Engine(game);
            var moves = ai.GetBestMoves();
            Assert.IsTrue(moves.Any(m => m.ToString() == "Black 24 -> 25"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "Black 21 -> 25"));
        }

        [TestMethod]
        public void BearOffEffectivelyWhite()
        {
            var game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 19);
            game.AddCheckers(1, Player.Color.White, 19);
            game.AddCheckers(3, Player.Color.Black, 20);
            game.AddCheckers(3, Player.Color.White, 20);
            game.AddCheckers(3, Player.Color.Black, 21);
            game.AddCheckers(3, Player.Color.White, 21);
            game.AddCheckers(3, Player.Color.Black, 22);
            game.AddCheckers(3, Player.Color.White, 22);
            game.AddCheckers(3, Player.Color.Black, 23);
            game.AddCheckers(3, Player.Color.White, 23);
            game.AddCheckers(2, Player.Color.Black, 24);
            game.AddCheckers(2, Player.Color.White, 24);

            game.FakeRoll(3, 2);
            game.SwitchPlayer();

            Assert.AreEqual(Player.Color.White, game.CurrentPlayer);
            var ai = new Ai.Engine(game);
            var moves = ai.GetBestMoves();
            Assert.IsTrue(moves.Any(m => m.ToString() == "White 23 -> 25"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "White 22 -> 25"));
        }

        [TestMethod]
        public void DontHitBlot1()
        {
            var game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 1);
            game.AddCheckers(1, Player.Color.Black, 2);
            game.AddCheckers(2, Player.Color.White, 20);

            game.AddCheckers(5, Player.Color.Black, 21);
            game.AddCheckers(5, Player.Color.Black, 23);
            game.AddCheckers(3, Player.Color.Black, 18);

            game.AddCheckers(3, Player.Color.White, 1);
            game.AddCheckers(3, Player.Color.White, 5);

            game.FakeRoll(4, 3);
            game.SwitchPlayer();

            Assert.AreEqual(Player.Color.White, game.CurrentPlayer);
            var ai = new Ai.Engine(game);
            var moves = ai.GetBestMoves();
            Assert.IsTrue(!moves.Any(m => m.To.WhiteNumber == 24));
            Assert.IsTrue(!moves.Any(m => m.To.WhiteNumber == 23));
        }

        [TestMethod]
        public void DontHitBlot2()
        {
            var game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 3);
            game.AddCheckers(1, Player.Color.Black, 4);
            game.AddCheckers(2, Player.Color.White, 18);

            game.AddCheckers(5, Player.Color.Black, 21);
            game.AddCheckers(5, Player.Color.Black, 23);
            game.AddCheckers(3, Player.Color.Black, 18);

            game.AddCheckers(3, Player.Color.White, 1);
            game.AddCheckers(3, Player.Color.White, 5);

            game.FakeRoll(4, 3);
            game.SwitchPlayer();

            Assert.AreEqual(Player.Color.White, game.CurrentPlayer);
            var ai = new Ai.Engine(game);
            var moves = ai.GetBestMoves();
            Assert.IsTrue(!moves.Any(m => m.To.WhiteNumber == 22));
            Assert.IsTrue(!moves.Any(m => m.To.WhiteNumber == 21));
        }

        [TestMethod]
        public void PlayersPassedTest()
        {
            var game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 19);
            game.AddCheckers(3, Player.Color.Black, 20);
            game.AddCheckers(3, Player.Color.Black, 21);
            game.AddCheckers(3, Player.Color.White, 22);
            game.AddCheckers(3, Player.Color.White, 23);
            game.AddCheckers(2, Player.Color.White, 24);
            Assert.IsTrue(game.PlayersPassed());

            game = Game.Create(false);
            game.ClearCheckers();
            game.AddCheckers(1, Player.Color.Black, 19);
            game.AddCheckers(3, Player.Color.Black, 20);
            game.AddCheckers(3, Player.Color.Black, 21);
            game.AddCheckers(3, Player.Color.White, 22);
            game.AddCheckers(3, Player.Color.White, 23);
            game.AddCheckers(2, Player.Color.White, 3);
            Assert.IsFalse(game.PlayersPassed());
        }
    }
}
