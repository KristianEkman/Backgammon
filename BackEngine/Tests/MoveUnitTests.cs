namespace Tests;

[TestClass]
public class MoveUnitTests
{
    [TestInitialize]
    public void Setup()
    {
        Engine = new();
    }
    private Engine Engine = new();

    [TestMethod]
    public void TestEngineStartUp()
    {
        Assert.AreEqual(Engine.Board.Spots[0], 0);
        Assert.AreEqual(Engine.Board.Spots[1], 2);
        Assert.AreEqual(Engine.Board.Spots[24], -2);
        Assert.AreEqual(Engine.Board.Spots[6], -5);
        Assert.AreEqual(Engine.Board.Spots[19], 5);
        Assert.AreEqual(Engine.Board.Spots[8], -3);
        Assert.AreEqual(Engine.Board.Spots[17], 3);
        Assert.AreEqual(Engine.Board.Spots[12], 5);
        Assert.AreEqual(Engine.Board.Spots[13], -5);
    }

    [TestMethod]
    public void TestMove()
    {
        Move whiteMove;
        whiteMove.From = 1;
        whiteMove.To = 3;
        whiteMove.Side = Engine.White;
        var blackHit = Engine.DoMove(whiteMove);
        Assert.AreEqual(1, Engine.Board.Spots[1]);
        Assert.AreEqual(1, Engine.Board.Spots[3]);
        Assert.IsFalse(blackHit);

        Move blackMove;
        blackMove.From = 24;
        blackMove.To = 22;
        blackMove.Side = Engine.Black;
        var whiteHit = Engine.DoMove(blackMove);
        Assert.AreEqual(-1, Engine.Board.Spots[24]);
        Assert.AreEqual(-1, Engine.Board.Spots[22]);
        Assert.IsFalse(whiteHit);

        Engine.UndoMove(blackMove, whiteHit);
        Assert.AreEqual(-2, Engine.Board.Spots[24]);
        Assert.AreEqual(0, Engine.Board.Spots[22]);

        Engine.UndoMove(whiteMove, blackHit);
        Assert.AreEqual(2, Engine.Board.Spots[1]);
        Assert.AreEqual(0, Engine.Board.Spots[3]);
    }

    [TestMethod]
    public void TestBlackHit()
    {
        Move blackMove;
        blackMove.From = 24;
        blackMove.To = 23;
        blackMove.Side = Engine.Black;
        _ = Engine.DoMove(blackMove);

        Move whiteMove;
        whiteMove.From = 19;
        whiteMove.To = 23;
        whiteMove.Side = Engine.White;
        var blackHit = Engine.DoMove(whiteMove);
        Assert.AreEqual(4, Engine.Board.Spots[19]);
        Assert.AreEqual(1, Engine.Board.Spots[23]);
        Assert.AreEqual(-1, Engine.Board.Spots[25]);
        Assert.IsTrue(blackHit);

        Engine.UndoMove(whiteMove, blackHit);
        Assert.AreEqual(5, Engine.Board.Spots[19]);
        Assert.AreEqual(-1, Engine.Board.Spots[23]);
        Assert.AreEqual(0, Engine.Board.Spots[25]);
    }

    [TestMethod]
    public void TestWhiteHit()
    {
        Move whiteMove;
        whiteMove.From = 1;
        whiteMove.To = 2;
        whiteMove.Side = Engine.White;
        _ = Engine.DoMove(whiteMove);

        Move blackMove;
        blackMove.From = 6;
        blackMove.To = 2;
        blackMove.Side = Engine.Black;
        var whiteHit = Engine.DoMove(blackMove);
        Assert.AreEqual(-4, Engine.Board.Spots[6]);
        Assert.AreEqual(-1, Engine.Board.Spots[2]);
        Assert.AreEqual(1, Engine.Board.Spots[0]);
        Assert.IsTrue(whiteHit);

        Engine.UndoMove(blackMove, whiteHit);
        Assert.AreEqual(-5, Engine.Board.Spots[6]);
        Assert.AreEqual(1, Engine.Board.Spots[2]);
        Assert.AreEqual(0, Engine.Board.Spots[0]);
    }

    [TestMethod]
    public void TestWhiteOff()
    {   
        for (int i = 0; i < 26; i++)
            Engine.Board.Spots[i] = 0;
        Engine.Board.Spots[1] = -3;
        Engine.Board.Spots[2] = -3;
        Engine.Board.Spots[3] = -3;
        Engine.Board.Spots[4] = -3;
        Engine.Board.Spots[5] = -3;

        Engine.Board.Spots[20] = 3;
        Engine.Board.Spots[21] = 3;
        Engine.Board.Spots[22] = 3;
        Engine.Board.Spots[23] = 3;
        Engine.Board.Spots[24] = 3;

        Move whiteMove;
        whiteMove.From = 20;
        whiteMove.To = 25;
        whiteMove.Side = Engine.White;
        var blackHit = Engine.DoMove(whiteMove);
        Assert.IsFalse(blackHit);
        Assert.AreEqual(0, Engine.Board.Spots[25]);
        Assert.AreEqual(1, Engine.Board.WhiteHome);
        Assert.AreEqual(2, Engine.Board.Spots[20]);

        Engine.UndoMove(whiteMove, blackHit);
        Assert.AreEqual(0, Engine.Board.Spots[25]);
        Assert.AreEqual(0, Engine.Board.WhiteHome);
        Assert.AreEqual(3, Engine.Board.Spots[20]);
    }

    [TestMethod]
    public void TestBlackOff()
    {
        for (int i = 0; i < 26; i++)
            Engine.Board.Spots[i] = 0;
        Engine.Board.Spots[1] = -3;
        Engine.Board.Spots[2] = -3;
        Engine.Board.Spots[3] = -3;
        Engine.Board.Spots[4] = -3;
        Engine.Board.Spots[5] = -3;

        Engine.Board.Spots[20] = 3;
        Engine.Board.Spots[21] = 3;
        Engine.Board.Spots[22] = 3;
        Engine.Board.Spots[23] = 3;
        Engine.Board.Spots[24] = 3;

        Move blackMove;
        blackMove.From = 3;
        blackMove.To = 0;
        blackMove.Side = Engine.Black;
        var whiteHit = Engine.DoMove(blackMove);
        Assert.IsFalse(whiteHit);
        Assert.AreEqual(0, Engine.Board.Spots[0]);
        Assert.AreEqual(1, Engine.Board.BlackHome);
        Assert.AreEqual(-2, Engine.Board.Spots[3]);

        Engine.UndoMove(blackMove, whiteHit);
        Assert.AreEqual(0, Engine.Board.Spots[0]);
        Assert.AreEqual(0, Engine.Board.BlackHome);
        Assert.AreEqual(-3, Engine.Board.Spots[3]);
    }
}
