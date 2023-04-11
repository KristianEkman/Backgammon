namespace Tests;

[TestClass]
public class MoveUnitTests
{
    [TestInitialize]
    public void Setup()
    {
        Board = new();
    }
    private Board Board = new();

    [TestMethod]
    public void TestEngineStartUp()
    {
        Assert.AreEqual(Board.Spots[0], 0);
        Assert.AreEqual(Board.Spots[1], 2);
        Assert.AreEqual(Board.Spots[24], -2);
        Assert.AreEqual(Board.Spots[6], -5);
        Assert.AreEqual(Board.Spots[19], 5);
        Assert.AreEqual(Board.Spots[8], -3);
        Assert.AreEqual(Board.Spots[17], 3);
        Assert.AreEqual(Board.Spots[12], 5);
        Assert.AreEqual(Board.Spots[13], -5);
    }

    [TestMethod]
    public void TestMove()
    {
        Assert.AreEqual((167, 167), (Board.WhitePip, Board.BlackPip));

        Move whiteMove;
        whiteMove.From = 1;
        whiteMove.To = 3;
        whiteMove.Side = Board.White;
        var blackHit = Board.DoMove(whiteMove);
        Assert.AreEqual((165, 167), (Board.WhitePip, Board.BlackPip));

        Assert.AreEqual(1, Board.Spots[1]);
        Assert.AreEqual(1, Board.Spots[3]);
        Assert.IsFalse(blackHit);

        Move blackMove;
        blackMove.From = 24;
        blackMove.To = 22;
        blackMove.Side = Board.Black;
        var whiteHit = Board.DoMove(blackMove);
        Assert.AreEqual((165, 165), (Board.WhitePip, Board.BlackPip));

        Assert.AreEqual(-1, Board.Spots[24]);
        Assert.AreEqual(-1, Board.Spots[22]);
        
        Assert.IsFalse(whiteHit);

        Board.UndoMove(blackMove, whiteHit);
        Assert.AreEqual((165, 167), (Board.WhitePip, Board.BlackPip));

        Assert.AreEqual(-2, Board.Spots[24]);
        Assert.AreEqual(0, Board.Spots[22]);

        Board.UndoMove(whiteMove, blackHit);
        Assert.AreEqual((167, 167), (Board.WhitePip, Board.BlackPip));

        Assert.AreEqual(2, Board.Spots[1]);
        Assert.AreEqual(0, Board.Spots[3]);
    }

    [TestMethod]
    public void TestBlackHit()
    {
        Move blackMove;
        blackMove.From = 24;
        blackMove.To = 23;
        blackMove.Side = Board.Black;
        _ = Board.DoMove(blackMove);
        Assert.AreEqual((167, 166), (Board.WhitePip, Board.BlackPip));


        Move whiteMove;
        whiteMove.From = 19;
        whiteMove.To = 23;
        whiteMove.Side = Board.White;
        var blackHit = Board.DoMove(whiteMove);
        Assert.AreEqual(4, Board.Spots[19]);
        Assert.AreEqual(1, Board.Spots[23]);
        Assert.AreEqual(-1, Board.Spots[25]);
        Assert.IsTrue(blackHit);
        Assert.AreEqual((163, 189), (Board.WhitePip, Board.BlackPip));

        Board.UndoMove(whiteMove, blackHit);
        Assert.AreEqual(5, Board.Spots[19]);
        Assert.AreEqual(-1, Board.Spots[23]);
        Assert.AreEqual(0, Board.Spots[25]);
        Assert.AreEqual((167, 166), (Board.WhitePip, Board.BlackPip));
    }

    [TestMethod]
    public void TestWhiteHit()
    {
        Move whiteMove;
        whiteMove.From = 1;
        whiteMove.To = 2;
        whiteMove.Side = Board.White;
        _ = Board.DoMove(whiteMove);
        Assert.AreEqual((166, 167), (Board.WhitePip, Board.BlackPip));

        Move blackMove;
        blackMove.From = 6;
        blackMove.To = 2;
        blackMove.Side = Board.Black;
        var whiteHit = Board.DoMove(blackMove);
        Assert.AreEqual(-4, Board.Spots[6]);
        Assert.AreEqual(-1, Board.Spots[2]);
        Assert.AreEqual(1, Board.Spots[0]);
        Assert.IsTrue(whiteHit);
        Assert.AreEqual((189, 163), (Board.WhitePip, Board.BlackPip));


        Board.UndoMove(blackMove, whiteHit);
        Assert.AreEqual(-5, Board.Spots[6]);
        Assert.AreEqual(1, Board.Spots[2]);
        Assert.AreEqual(0, Board.Spots[0]);
        Assert.AreEqual((166, 167), (Board.WhitePip, Board.BlackPip));
    }

    [TestMethod]
    public void TestWhiteOff()
    {   
        for (int i = 0; i < 26; i++)
            Board.Spots[i] = 0;
        Board.Spots[1] = -3;
        Board.Spots[2] = -3;
        Board.Spots[3] = -3;
        Board.Spots[4] = -3;
        Board.Spots[5] = -3;

        Board.Spots[20] = 3;
        Board.Spots[21] = 3;
        Board.Spots[22] = 3;
        Board.Spots[23] = 3;
        Board.Spots[24] = 3;

        Move whiteMove;
        whiteMove.From = 20;
        whiteMove.To = 25;
        whiteMove.Side = Board.White;
        var blackHit = Board.DoMove(whiteMove);
        Assert.IsFalse(blackHit);
        Assert.AreEqual(0, Board.Spots[25]);
        Assert.AreEqual(1, Board.WhiteHome);
        Assert.AreEqual(2, Board.Spots[20]);

        Board.UndoMove(whiteMove, blackHit);
        Assert.AreEqual(0, Board.Spots[25]);
        Assert.AreEqual(0, Board.WhiteHome);
        Assert.AreEqual(3, Board.Spots[20]);
    }

    [TestMethod]
    public void TestBlackOff()
    {
        for (int i = 0; i < 26; i++)
            Board.Spots[i] = 0;
        Board.Spots[1] = -3;
        Board.Spots[2] = -3;
        Board.Spots[3] = -3;
        Board.Spots[4] = -3;
        Board.Spots[5] = -3;

        Board.Spots[20] = 3;
        Board.Spots[21] = 3;
        Board.Spots[22] = 3;
        Board.Spots[23] = 3;
        Board.Spots[24] = 3;

        Move blackMove;
        blackMove.From = 3;
        blackMove.To = 0;
        blackMove.Side = Board.Black;
        var whiteHit = Board.DoMove(blackMove);
        Assert.IsFalse(whiteHit);
        Assert.AreEqual(0, Board.Spots[0]);
        Assert.AreEqual(1, Board.BlackHome);
        Assert.AreEqual(-2, Board.Spots[3]);

        Board.UndoMove(blackMove, whiteHit);
        Assert.AreEqual(0, Board.Spots[0]);
        Assert.AreEqual(0, Board.BlackHome);
        Assert.AreEqual(-3, Board.Spots[3]);
    }
}
