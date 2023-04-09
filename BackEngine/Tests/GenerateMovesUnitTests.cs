using System.Diagnostics;

namespace Tests;

[TestClass]
public class GenerateMovesUnitTests
{
    [TestInitialize]
    public void Setup()
    {
        Engine = new();
    }
    private Engine Engine = new();

    [TestMethod]
    public void White1()
    {
        var gen = new Generation(2, 1);
        Engine.CreateMoves(gen, Engine.White);
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
        Engine.CreateMoves(gen, Engine.White);
        Assert.AreEqual(42, gen.GeneratedCount);
        gen.PrintMoves();
    }

    [TestMethod]
    public void Black1()
    {
        var gen = new Generation(2, 1);
        Engine.CreateMoves(gen, Engine.Black);

        gen.PrintMoves();

        Assert.AreEqual(18, gen.GeneratedCount);

    }

    [TestMethod]
    public void BlackDouble()
    {
        var gen = new Generation(2, 2);
        Engine.CreateMoves(gen, Engine.Black);
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
    public void WhiteHit()
    {
        Engine.Board.Spots[0] = 2;
        Engine.Board.Spots[24] = 0;
        var gen = new Generation(1, 3);
        Engine.CreateMoves(gen, Engine.White);

        Assert.IsTrue(gen.MoveSets.Take(gen.GeneratedCount).ToArray()
            .All(m => m.Take(2).All(x => x.From == 0)));}

    [TestMethod]
    public void BlackHit()
    {
        Engine.Board.Spots[25] = -2;
        Engine.Board.Spots[1] = 0;
        var gen = new Generation(1, 3);
        Engine.CreateMoves(gen, Engine.Black);
        gen.PrintMoves();

        Assert.IsTrue(gen.MoveSets.Take(gen.GeneratedCount).ToArray()
            .All(m => m.Take(2).All(x => x.From == 25)));
    }

    [TestMethod]
    public void White56()
    {
        var gen = new Generation(5, 6);
        Engine.CreateMoves(gen, Engine.White);
        gen.PrintMoves();
    }

    [TestMethod]
    public void BearOff()
    {
        var gen = new Generation(5, 6);
        Engine.Clear();
        Engine.Board.Spots[20] = 3;
        Engine.Board.Spots[21] = 3;
        Engine.Board.Spots[22] = 3;
        Engine.Board.Spots[23] = 3;
        Engine.Board.Spots[24] = 3;

        Engine.Board.Spots[1] = -3;
        Engine.Board.Spots[2] = -3;
        Engine.Board.Spots[3] = -3;
        Engine.Board.Spots[4] = -3;
        Engine.Board.Spots[5] = -3;

        Engine.CreateMoves(gen, Engine.White);
        gen.PrintMoves();

        Assert.AreEqual(1, gen.GeneratedCount);
        Assert.IsTrue(gen.MoveSets.Take(1).All(ms => ms.Take(2).All( m => m.ToString() == "20-25 White")));

        Engine.CreateMoves(gen, Engine.Black);
        gen.PrintMoves();

        Assert.AreEqual(1, gen.GeneratedCount);
        Assert.IsTrue(gen.MoveSets.Take(1).All(ms => ms.Take(2).All(m => m.ToString() == "5-0 Black")));
    }
}
