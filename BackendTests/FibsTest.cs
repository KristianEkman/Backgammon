using FibsIntegration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests
{
    [TestClass]
    public class FibsTest
    {
        [TestMethod]
        public void TestConnect()
        {
            var fibs = new Fibs();
            fibs.Connect();
            fibs.Disconnect();
        }

        [TestMethod]
        public void TestParse()
        {
            var raw = "board:You:udacity_capstone:1:0:0:0:-1:0:0:0:0:5:-1:3:0:0:0:-4:5:-1:0:0:-3:0:-5:0:0:0:0:2:0:1:2:5:0:0:1:1:1:0:1:-1:0:25:0:0:0:0:2:0:0:0";
            var board = Board.Parse(raw);
            Assert.AreEqual("You", board.PlayerName);
            Assert.AreEqual("udacity_capstone", board.OpponentName);
        }

        [TestMethod]
        public void TestToInternal()
        {
            var raw = "board:You:udacity_capstone:1:0:0:0:-1:0:0:0:0:5:-1:3:0:0:0:-4:5:-1:0:0:-3:0:-5:0:0:0:0:2:0:1:2:5:0:0:1:1:1:0:1:-1:0:25:0:0:0:0:2:0:0:0";
            var board = Board.Parse(raw);
            var s = board.ToInternal();
            Assert.AreEqual("0 w1 0 0 0 0 b5 w1 b3 0 0 0 w4 b5 w1 0 0 w3 0 w5 0 0 0 0 b2 0 0 0 b 2 5", s);
        }

        [TestMethod]
        public void TestParseBoard()
        {
            // use to debug boards
            var raw = "board:You:MonteCarlo:1:0:0:0:0:7:5:1:1:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:0:-4:0:1:0:0:0:0:1:1:1:0:1:-1:0:25:1:11:0:0:2:5:0:0";
            var board = Board.Parse(raw);

        }

    }
}
