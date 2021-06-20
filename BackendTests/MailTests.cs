using Backend.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendTests
{
    [TestClass]
    public class MailTest
    {
        [TestMethod]
        [Ignore]
        public void TestMail()
        {
            Mailer.SendTest();
        }
    }
}