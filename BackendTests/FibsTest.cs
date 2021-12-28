﻿using FibsIntegration;
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
        // todo:

    }
}