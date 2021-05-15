using Backend.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MailTest
{
    [TestMethod]
    public void TestMail()
    {
        Mailer.SendTest();
    }
}