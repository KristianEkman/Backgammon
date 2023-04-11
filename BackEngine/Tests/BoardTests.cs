using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Numerics;

namespace Tests;

[TestClass]
public class BoardTests
{
    [TestInitialize]
    public void Setup()
    {
        Board = new();
    }
    private Board Board = new();

    [TestMethod]
    public void WhiteMoves1()
    {
        var gen = new Generation(2, 1);
        Board.CreateMoves(gen, Board.White);
        gen.PrintMoves();

        Assert.AreEqual(18, gen.GeneratedCount);

        // 1 - 2, 1 - 3
        // 1 - 2, 2 - 4
        // 1 - 2, 12 -14
        // 1 - 2, 17 - 19
        // 1 - 2, 19 - 21 

        // 1 - 3, 3 - 4
        // 1 - 3, 17 - 18
        // 1 - 3, 19 - 20

        // 12 - 14, 14 - 15
        // 12 - 14, 17 - 18
        // 12 - 14, 19 - 20

        // 17 - 18, 17 - 19
        // 17 - 18, 18 - 20
        // 17 - 18, 19 - 21

        // 17 - 19, 19 - 20

        // 19 - 20, 19 - 21
        // 19 - 20, 20 - 22

        // froms1:  1, 12, 17, 19
        // tos1:    2,3,14,18,19,20

    }

    [TestMethod]
    public void WhiteDouble()
    {
        var gen = new Generation(2, 2);
        Board.CreateMoves(gen, Board.White);
        Assert.AreEqual(42, gen.GeneratedCount);
        gen.PrintMoves();
    }

    [TestMethod]
    public void BlackMoves1()
    {
        var gen = new Generation(2, 1);
        Board.CreateMoves(gen, Board.Black);
        gen.PrintMoves();
        Assert.AreEqual(18, gen.GeneratedCount);
    }

    [TestMethod]
    public void BlackDouble()
    {
        var gen = new Generation(2, 2);
        Board.CreateMoves(gen, Board.Black);
        Assert.AreEqual(42, gen.GeneratedCount);

        for (int i = 0; i < gen.GeneratedCount; i++)
        {
            for (int j = 0; j < gen.Dice.Length; j++)
            {
                Debug.Write(gen.MoveSets[i][j]);
                Debug.Write("   ");
            }
            Debug.WriteLine("");
        }
    }
    
    [TestMethod]
    public void White56()
    {
        var gen = new Generation(5, 6);
        Board.CreateMoves(gen, Board.White);
        gen.PrintMoves();
    }

    [TestMethod]
    public void BearOff()
    {
        var gen = new Generation(5, 6);
        Board.Clear();
        Board.Spots[20] = 3;
        Board.Spots[21] = 3;
        Board.Spots[22] = 3;
        Board.Spots[23] = 3;
        Board.Spots[24] = 3;

        Board.Spots[1] = -3;
        Board.Spots[2] = -3;
        Board.Spots[3] = -3;
        Board.Spots[4] = -3;
        Board.Spots[5] = -3;        
        Board.CreateMoves(gen, Board.White);
        gen.PrintMoves();

        Assert.AreEqual(1, gen.GeneratedCount);
        Assert.IsTrue(gen.MoveSets.Take(1).All(ms => ms.Take(2).All( m => m.ToString() == "20-25 White")));

        Board.CreateMoves(gen, Board.Black);
        gen.PrintMoves();

        Assert.AreEqual(1, gen.GeneratedCount);
        Assert.IsTrue(gen.MoveSets.Take(1).All(ms => ms.Take(2).All(m => m.ToString() == "5-0 Black")));
    }

    [TestMethod]
    public void PlayBothDiceIfPossible()
    {
        Board.Clear();

        Board.Spots[1] = 2;
        Board.Spots[12] = 5;
        Board.Spots[18] = 3;
        Board.Spots[19] = 5;
        
        Board.Spots[23] = -2;
        Board.Spots[22] = -2;
        Board.Spots[16] = -2;
        Board.Spots[6] = -3;
        Board.Spots[5] = -2;
        Board.Spots[3] = -2;
        Board.Spots[2] = -2;

        var gen = new Generation(4, 6);
        Board.CreateMoves(gen, Board.White);
        gen.PrintMoves();
        Assert.AreEqual(1, gen.GeneratedCount);

        Assert.IsFalse(gen.MoveSets.Any(ms => ms.Any(m => m.From == 12 && m.To == 18)));
    }

    [TestMethod]
    public void OnlyOneDicePlayable()
    {
        Board.Clear();

        Board.Spots[1] = 2;
        Board.Spots[2] = -2;
        Board.Spots[3] = -2;
        Board.Spots[5] = -2;
        Board.Spots[6] = -3;

        Board.Spots[7] = 1;
        Board.Spots[16] = -2;
        Board.Spots[19] = 8;
        Board.Spots[23] = -2;
        Board.Spots[24] = -2;

        Board.WhiteHome = 4;


        var gen = new Generation(4, 5);
        Board.CreateMoves(gen, Board.White);
        gen.PrintMoves();
        Assert.AreEqual(2, gen.GeneratedCount);
        Assert.AreEqual("empty", gen.MoveSets[0][1].ToString());
        Assert.AreEqual("empty", gen.MoveSets[1][1].ToString());
    }

    [TestMethod]
    public void BoardScoreBlackBlot()
    {
        Board.Spots[24] = -1;
        Board.Spots[23] = -1;
        Board.CountPips();
        var score = Board.GetScore();
        Assert.IsTrue(score > 0);
    }

    [TestMethod]
    public void BoardScoreWhiteBlot()
    {
        Board.Spots[1] = 1;
        Board.Spots[2] = 1;
        Board.CountPips();

        var score = Board.GetScore();
        Assert.IsTrue(score < 0);
    }

    [TestMethod]
    public void FirstWhite()
    {
        Assert.AreEqual(1, Board.FirstWhite);
        Move move;
        move.From = 1;
        move.Side = Board.White;
        move.To = 4;
        
        var undid1 = Board.DoMove(move);
        Assert.AreEqual(1, Board.FirstWhite);

        var undid2 = Board.DoMove(move);
        Assert.AreEqual(4, Board.FirstWhite);

        Board.UndoMove(move, undid2);
        Assert.AreEqual(1, Board.FirstWhite);

        Board.UndoMove(move, undid1);
        Assert.AreEqual(1, Board.FirstWhite);

    }

    [TestMethod]
    public void FirstBlack()
    {
        Assert.AreEqual(24, Board.FirstBlack);
        Move move;
        move.From = 24;
        move.Side = Board.Black;
        move.To = 21;

        var undid1 = Board.DoMove(move);
        Assert.AreEqual(24, Board.FirstBlack);

        var undid2 = Board.DoMove(move);
        Assert.AreEqual(21, Board.FirstBlack);

        Board.UndoMove(move, undid2);
        Assert.AreEqual(24, Board.FirstBlack);

        Board.UndoMove(move, undid1);
        Assert.AreEqual(24, Board.FirstBlack);
    }

    [TestMethod]
    public void FirstWithHitWhite()
    {
        Move move;
        move.From = 1;
        move.To = 2;
        move.Side = Board.White;
        Board.DoMove(move);
        Assert.AreEqual(1, Board.FirstWhite);

        Move blackMove;
        blackMove.From = 6;
        blackMove.To = 2;
        blackMove.Side = Board.Black;

        var undid = Board.DoMove(blackMove);
        Assert.IsTrue(undid.Hit);

        Assert.AreEqual(0, Board.FirstWhite);

        Board.UndoMove(blackMove, undid);
        Assert.AreEqual(1, Board.FirstWhite);
    }

    [TestMethod]
    public void FirstWithHitBlack()
    {
        Move move;
        move.From = 24;
        move.To = 23;
        move.Side = Board.Black;
        Board.DoMove(move);
        Assert.AreEqual(24, Board.FirstBlack);

        Move whiteMove;
        whiteMove.From = 19;
        whiteMove.To = 23;
        whiteMove.Side = Board.White;

        var undid = Board.DoMove(whiteMove);
        Assert.IsTrue(undid.Hit);

        Assert.AreEqual(25, Board.FirstBlack);

        Board.UndoMove(whiteMove, undid);
        Assert.AreEqual(24, Board.FirstBlack);
    }
}
