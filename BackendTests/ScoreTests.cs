using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests
{
    [TestClass]
    public class ScoreTests
    {
        [TestMethod]
        public void TestScore()
        {
            var black = 1600;
            var white = 1600;
            var computed = Score.NewScore(black, white, 30, 30, true);
            Assert.AreEqual(1610, computed.black);
            Assert.AreEqual(1590, computed.white);

            computed = Score.NewScore(1400, 1900, 300, 300, true);
            Assert.AreEqual(1414, computed.black);
            Assert.AreEqual(1886, computed.white);

            computed = Score.NewScore(1400, 1900, 5, 5, true);
            Assert.AreEqual(1463, computed.black);
            Assert.AreEqual(1837, computed.white);

            computed = Score.NewScore(1400, 1900, 5, 5, false);
            Assert.AreEqual(1396, computed.black);
            Assert.AreEqual(1904, computed.white);
        }
    }
}
