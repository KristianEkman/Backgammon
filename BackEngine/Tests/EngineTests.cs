using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public void TestBesMove()
    {
        var set1  = Engine.GetBestMoveSet(1, 6, Board.White);
        Assert.IsTrue(set1.Any(s => s.From == 12 && s.To == 18));
        Assert.IsTrue(set1.Any(s => s.From == 17 && s.To == 18));

        var set2 = Engine.GetBestMoveSet(1, 6, Board.Black);
        Assert.IsTrue(set2.Any(s => s.From == 13 && s.To == 7));
        Assert.IsTrue(set2.Any(s => s.From == 8 && s.To == 7));
    }
}