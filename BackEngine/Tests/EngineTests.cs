using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;
[TestClass]
public class EngineTests
{
    private Engine Engine = new();

    [TestInitialize]
    public void Init()
    {
        Engine = new Engine();
    }

    [TestMethod]
    public void TestBesMove1()
    {
        var set1 = Engine.GetBestMoveSet(1, 6, Board.White);
        Assert.IsTrue(set1.Any(s => s.From == 12 && s.To == 18));
        Assert.IsTrue(set1.Any(s => s.From == 17 && s.To == 18));

        var set2 = Engine.GetBestMoveSet(1, 6, Board.Black);
        Assert.IsTrue(set2.Any(s => s.From == 13 && s.To == 7));
        Assert.IsTrue(set2.Any(s => s.From == 8 && s.To == 7));
    }

    [TestMethod]
    public void TestBesMoveDoubleWhite()
    {
        var spots = Engine.Board.Spots;
        spots[17] = 4;
        spots[12] = 4;
        var set1 = Engine.GetBestMoveSet(1, 1, Board.White);
        Console.WriteLine(set1[0]);
        Console.WriteLine(set1[1]);
        Console.WriteLine(set1[2]);
        Console.WriteLine(set1[3]);

    }
}