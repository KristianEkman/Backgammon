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
            var game = Game.Create();
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
            var game = Game.Create();
            var ai = new Ai.Engine(game);
            game.ClearCheckers();

            game.AddCheckers(1, Player.Color.Black, 1);
            game.AddCheckers(1, Player.Color.White, 1);

            game.AddCheckers(1, Player.Color.Black, 2);
            game.AddCheckers(1, Player.Color.White, 2);

            game.AddCheckers(3, Player.Color.Black, 17);
            game.AddCheckers(3, Player.Color.White, 17);

            game.AddCheckers(1, Player.Color.Black, 19); // 6 for white, the target
            game.AddCheckers(1, Player.Color.White, 19);

            game.FakeRoll(4, 5);
            game.SwitchPlayer();
            var moves = ai.GetBestMoves();
            Assert.IsTrue(moves.SingleOrDefault(m => m.ToString() == "White 1 -> 6") != null);
            Assert.IsTrue(moves.SingleOrDefault(m => m.ToString() == "White 2 -> 6") != null);
        }

        [TestMethod]
        public void DoubleDice()
        {
            var game = Game.Create();
            var ai = new Ai.Engine(game);
            game.FakeRoll(2, 2);
            var moves = ai.GetBestMoves();

        }

        [TestMethod]
        public void FirstDiceBlocked()
        {
            var game = Game.Create();
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

    }
}
