using System;
using csrun.adapters.providers;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class App_tests
    {
        [SetUp]
        public void Setup() {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }
        
        
        [Test]
        public void Test()
        {
            var fs = new Filesystem();
            var fl = new FailureLog();
            var sut = new App(fs, fl);
            
            var output = ConsoleOutput.Capture(() => sut.Execute(new CLI.RunCommand("app_test.csrun")));
            
            Assert.AreEqual("1+2=3", output.Trim());
        }
    }
}