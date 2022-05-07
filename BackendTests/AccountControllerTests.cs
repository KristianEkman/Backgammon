using Backend.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendTests;
[TestClass]
public class AccountControllerTests
{
    [TestMethod]
    public void AcceptedLanguagesOneLangTests()
    {
        var data = "en-US,en;q=0.5";
        var langs = AccountControllerExtensions.ParseLanguages(data);
        Assert.AreEqual(1, langs.Length);
        Assert.AreEqual("en", langs[0]);

        data = "en-US,sv-SE;q=0.5";
        langs = AccountControllerExtensions.ParseLanguages(data);
        Assert.AreEqual(2, langs.Length);
        Assert.AreEqual("en", langs[0]);
        Assert.AreEqual("sv", langs[1]);

        data = "en-US,sv-SE";
        langs = AccountControllerExtensions.ParseLanguages(data);
        Assert.AreEqual(2, langs.Length);
        Assert.AreEqual("en", langs[0]);
        Assert.AreEqual("sv", langs[1]);

        data = "en,sv";
        langs = AccountControllerExtensions.ParseLanguages(data);
        Assert.AreEqual(2, langs.Length);
        Assert.AreEqual("en", langs[0]);
        Assert.AreEqual("sv", langs[1]);
    }
}
